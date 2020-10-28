using Pl.Crawler.Core.Interfaces;
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Constants;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Pl.Crawler.Core.Services
{
    public class FindlinkService : IServiceStart
    {
        private static readonly Dictionary<string, bool> webPageKey = new Dictionary<string, bool>();
        private readonly IWebpageData webpageData;
        private readonly IMesssageQueueService messsageQueueService;
        private readonly ICacheCrawlConfigsService cacheGetCrawlConfigService;
        private readonly IWebpageContentData webpageContentData;

        public FindlinkService(
            IWebpageContentData _webpageContentData,
            ICacheCrawlConfigsService _cacheGetCrawlConfigService,
            IMesssageQueueService _messsageQueueService,
            IWebpageData _webpageData)
        {
            webpageData = _webpageData;
            messsageQueueService = _messsageQueueService;
            cacheGetCrawlConfigService = _cacheGetCrawlConfigService;
            webpageContentData = _webpageContentData;
        }

        public void StartService()
        {
            messsageQueueService.RegisterConsoleWorker(MessageQueueConstants.FindlinkWebContentTaskQueueName, (message) =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                FindNewUrl(message);

                stopWatch.Stop();
                $"Time find link page Id {message} in {stopWatch.ElapsedMilliseconds} miniseconds".WriteConsole(ConsoleColor.Yellow);
                return true;
            });
        }

        /// <summary>
        /// tìm url mới
        /// </summary>
        /// <param name="webpageId">Id trang web đã download</param>
        public void FindNewUrl(string webpageId)
        {
            var webpage = webpageData.FindByKey(webpageId);
            if (webpage == null || webpage.ErrorDownloadCount > 0)
            {
                return;
            }

            var webcontent = webpageContentData.GetContent(webpage.Id);
            if (string.IsNullOrEmpty(webcontent))
            {
                return;
            }

            var configs = cacheGetCrawlConfigService.GetCrawlConfigsByWebsiteId(webpage.WebsiteId);
            if (configs?.Count <= 0)
            {
                return;
            }

            try
            {
                var baseUri = new Uri(webpage.Url);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(webcontent);
                document.OptionOutputAsXml = true;
                document.OptionAutoCloseOnEnd = true;
                document.OptionFixNestedTags = true;
                var links = document.DocumentNode.Descendants().Where(n => n.Name == "a").ToList();
                var webpages = new List<Webpage>();

                foreach (var link in links)
                {
                    var tagUrl = link.GetAttributeValue("href", "");
                    if (string.IsNullOrEmpty(tagUrl) || tagUrl.StartsWith("#"))
                    {
                        continue;
                    }

                    if (!Uri.TryCreate(baseUri, tagUrl, out Uri tagUri) || !(tagUri.Scheme == Uri.UriSchemeHttp || tagUri.Scheme == Uri.UriSchemeHttps))
                    {
                        continue;
                    }

                    if (tagUri.Host != baseUri.Host)
                    {
                        continue;
                    }

                    var checkUrl = tagUri.AbsoluteUri;
                    foreach (var config in configs)
                    {
                        var urlMatch = Regex.Match(checkUrl, config.UrlPattern);
                        if (urlMatch == null || !urlMatch.Success || urlMatch.Value != checkUrl)
                        {
                            continue;
                        }

                        using (var md5Creater = MD5.Create())
                        {
                            var md5Url = Utility.GetMd5Hash(md5Creater, checkUrl);
                            var keyCheck = $"{md5Url}_{webpage.WebsiteId}";

                            if (!webPageKey.ContainsKey(keyCheck))
                            {
                                webPageKey.Add(keyCheck, webpageData.IsExistLink(md5Url));
                            }

                            if (!webPageKey[keyCheck])
                            {
                                webpages.Add(new Webpage()
                                {
                                    Url = checkUrl,
                                    UrlMd5 = md5Url,
                                    NextDownloadTime = DateTime.Now.AddSeconds(30),
                                    WebsiteId = webpage.WebsiteId,
                                    ConfigId = config.Id
                                });
                                webPageKey[keyCheck] = true;
                            }
                        }
                    }
                }

                if (webpages.Count > 0)
                {
                    webpageData.Insert(webpages);
                    webpage.NoNewLinkFoundCount = 0;
                    webpage.DownloadRank = webpage.DownloadRank < 8 ? webpage.DownloadRank + 1 : webpage.DownloadRank;
                }
                else
                {
                    webpage.NoNewLinkFoundCount += 1;
                    if (webpage.NoNewLinkFoundCount > CrawlConstants.MaxNoNewLinkFindCount && webpage.DownloadRank < 9)//những page có rank từ 9 trở lên sẽ không bao giờ bị tụt rank
                    {
                        webpage.DownloadRank = webpage.DownloadRank > 0 ? webpage.DownloadRank - 1 : 0;
                        webpage.NoNewLinkFoundCount = 0;
                    }
                }
                webpageData.Update(webpage);

                $"Find {webpages.Count} on url webpage url: {webpage.Url} ".WriteConsole(ConsoleColor.Blue);
            }
            catch (Exception ex)
            {
                $"Error find webpage url: {webpage.Url}".WriteConsole(ConsoleColor.Blue);
                ex.Message.WriteConsole(ConsoleColor.Red);
            }
        }

    }
}
