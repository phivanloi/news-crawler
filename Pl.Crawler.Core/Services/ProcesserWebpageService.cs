using Pl.Crawler.Core.Interfaces;
using System;
using System.Collections.Generic;
using Pl.Crawler.Core.Constants;
using HtmlAgilityPack;
using System.Linq;
using Pl.Crawler.Core.Entities;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Pl.Crawler.Core.Services
{
    public class ProcesserWebpageService : IServiceStart
    {
        private readonly IWebpageData webpageData;
        private readonly IDataExportData dataExportData;
        private readonly IWebpageContentData webpageContentData;
        private readonly IMesssageQueueService messsageQueueService;
        private readonly ICacheCrawlConfigsService cacheGetCrawlConfigService;

        public ProcesserWebpageService(
            IWebpageContentData _webpageContentData,
            ICacheCrawlConfigsService _cacheGetCrawlConfigService,
            IDataExportData _dataExportData,
            IMesssageQueueService _messsageQueueService,
            IWebpageData _webpageData)
        {
            webpageData = _webpageData;
            messsageQueueService = _messsageQueueService;
            dataExportData = _dataExportData;
            cacheGetCrawlConfigService = _cacheGetCrawlConfigService;
            webpageContentData = _webpageContentData;
        }

        public void StartService()
        {
            messsageQueueService.RegisterConsoleWorker(MessageQueueConstants.ProcessWebContentTaskQueueName, (message) =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                PaserPage(message);

                stopWatch.Stop();
                $"Paser page Id {message} in {stopWatch.ElapsedMilliseconds} miniseconds".WriteConsole(ConsoleColor.Yellow);
                return true;
            });
        }

        /// <summary>
        /// Phân tích 
        /// </summary>
        /// <param name="webpageId"></param>
        /// <returns>Id data export nếu thành công</returns>
        public void PaserPage(string webpageId)
        {
            var webpage = webpageData.FindByKey(webpageId);
            if (webpage == null)
            {
                return;
            }

            var webcontent = webpageContentData.GetContent(webpage.Id);
            if (string.IsNullOrEmpty(webcontent))
            {
                return;
            }

            try
            {
                $"Start Paser webpage url: {webpage.Url}".WriteConsole(ConsoleColor.Blue);
                var isUpdateOrNew = true;
                var idDataExport = "";
                var crawlConfigs = cacheGetCrawlConfigService.GetCrawlConfigsByWebsiteId(webpage.WebsiteId);
                foreach (var crawlConfig in crawlConfigs)
                {
                    var urlMatch = Regex.Match(webpage.Url, crawlConfig.UrlPattern);
                    if (urlMatch != null && urlMatch.Success && urlMatch.Value == webpage.Url)
                    {
                        var paserFields = cacheGetCrawlConfigService.GetParseFieldsByCrawlConfigId(crawlConfig.Id);
                        if (paserFields?.Count() > 0)
                        {
                            var keyPerValue = new Dictionary<string, string>();
                            foreach (var paserField in paserFields)
                            {
                                var selectRules = cacheGetCrawlConfigService.GetSelectRulesByPaserFieldId(paserField.Id);
                                var replateRules = cacheGetCrawlConfigService.GetReplateRulesByPaserFieldId(paserField.Id);
                                var paserValue = "";

                                foreach (var selectRule in selectRules)
                                {
                                    paserValue = PaserField(webcontent, selectRule);
                                    if (!string.IsNullOrEmpty(paserValue))
                                    {
                                        break;
                                    }
                                }

                                foreach (var replateRule in replateRules)
                                {
                                    paserValue = ReplateData(paserValue, replateRule);
                                }

                                if (!keyPerValue.ContainsKey(paserField.FieldName))
                                {
                                    keyPerValue.Add(paserField.FieldName, paserValue);
                                }
                            }

                            var paserData = JsonConvert.SerializeObject(keyPerValue);
                            var checkKey = $"exportkeycheck-{webpage.WebsiteId}-{crawlConfig.Id}-{webpage.Id}";
                            var dataExport = dataExportData.Find(q => q.ExistKey == checkKey);
                            if (dataExport != null)
                            {
                                dataExport.IsWarningData = keyPerValue.Any(q => string.IsNullOrEmpty(q.Value));
                                dataExport.IsUpdate = dataExport.PaserData != paserData;
                                dataExport.PaserData = paserData;
                                dataExport.LastPaserTime = DateTime.Now;
                                if (crawlConfig.AutoExport && dataExport.IsUpdate)
                                {
                                    var checkUpdate = dataExportData.Update(dataExport);
                                    if (checkUpdate)
                                    {
                                        messsageQueueService.PublishTask(MessageQueueConstants.DataExportTaskQueueName, dataExport.Id);
                                    }
                                }
                                if (!isUpdateOrNew)
                                {
                                    isUpdateOrNew = dataExport.IsUpdate;
                                }
                            }
                            else
                            {
                                dataExport = new DataExport()
                                {
                                    CrawlConfigId = crawlConfig.Id,
                                    WebpageId = webpage.Id,
                                    PaserData = paserData,
                                    WebsiteId = webpage.WebsiteId,
                                    IsWarningData = keyPerValue.Any(q => string.IsNullOrEmpty(q.Value)),
                                    ExistKey = checkKey
                                };
                                var checkInsert = dataExportData.Insert(dataExport);
                                if (checkInsert && crawlConfig.AutoExport)
                                {
                                    messsageQueueService.PublishTask(MessageQueueConstants.DataExportTaskQueueName, dataExport.Id);
                                }
                                isUpdateOrNew = true;
                            }
                            idDataExport = dataExport.Id;

                        }
                    }
                }

                webpage.LastProcessedTime = DateTime.Now;
                webpage.ErrorProcessCount = 0;
                webpage.ProcessNoUpdateCount = isUpdateOrNew ? 0 : webpage.ProcessNoUpdateCount + 1;
                if (webpage.ProcessNoUpdateCount >= CrawlConstants.MaxProcessNoUpdateCount && webpage.DownloadRank < 9)//những page có rank từ 9 trở lên sẽ không bao giờ bị tụt rank
                {
                    webpage.DownloadRank = webpage.DownloadRank > 0 ? webpage.DownloadRank - 1 : 0;
                    webpage.ProcessNoUpdateCount = 0;
                }
                webpage.NextDownloadTime = webpage.NextDownloadTime.AddSeconds(webpageData.GetTimeNextDownloadByRank(webpage.DownloadRank, webpage.CreatedTime));
            }
            catch (Exception ex)
            {
                $"Error paser webpage url: {webpage.Url}".WriteConsole(ConsoleColor.Red);
                ex.Message.WriteConsole(ConsoleColor.Red);
                webpage.ErrorProcessCount += 1;
                webpage.NextDownloadTime = webpage.NextDownloadTime.AddSeconds(webpageData.GetTimeNextDownloadByRank(webpage.DownloadRank, webpage.CreatedTime) + CrawlConstants.ErrorDownloadTimeAppend);
            }
            finally
            {
                webpageData.Update(webpage);
            }
        }

        /// <summary>
        /// Chạy vòng lặp xử lý thay thế dữ liệu
        /// </summary>
        /// <param name="source">Dữ liệu sau khi phân tích xong</param>
        /// <param name="replateRule">Định nghĩa cách thay thế dữ liệu</param>
        /// <returns>string</returns>
        public string ReplateData(string source, ReplateRule replateRule)
        {
            try
            {
                string replateString = "";
                if (!string.IsNullOrWhiteSpace(replateRule.ReplaceData))
                {
                    replateString = replateRule.ReplaceData;
                }

                if (replateRule.ParserType == CrawlConstants.PaserFieldName.ParserTypeRegex)
                {
                    return Regex.Replace(source, replateRule.SelectKey, replateString, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
                }
                if (replateRule.ParserType == CrawlConstants.PaserFieldName.ParserTypeXpath)
                {
                    HtmlDocument htmlDocument = new HtmlDocument
                    {
                        OptionAutoCloseOnEnd = true
                    };
                    htmlDocument.LoadHtml(source);
                    HtmlNode htmlNode = htmlDocument.DocumentNode.SelectNodes(replateRule.SelectKey)?.FirstOrDefault();
                    if (htmlNode != null)
                    {
                        if (string.IsNullOrWhiteSpace(replateRule.ReplaceData))
                        {
                            htmlNode.ParentNode.RemoveChild(htmlNode);
                        }
                        else
                        {
                            htmlNode.ParentNode.ReplaceChild(HtmlNode.CreateNode(replateString), htmlNode);
                        }
                    }
                    return htmlDocument.DocumentNode.OuterHtml;
                }
                return source;
            }
            catch (Exception ex)
            {
                ex.Message.WriteConsole(ConsoleColor.Red);
                return source;
            }
        }

        /// <summary>
        /// Xử lý paser một Field từ nội dung source được cung cấp
        /// </summary>
        /// <param name="itemString">Nội dung string để paser</param>
        /// <param name="selectRule">Định nghĩa cách paser một field</param>
        /// <returns>string</returns>
        public string PaserField(string itemString, SelectRule selectRule)
        {
            string setValue = "";
            if (selectRule.IsDefault)
            {
                setValue = selectRule.DefaultData;
            }
            else
            {
                if (string.IsNullOrEmpty(itemString))
                {
                    return string.Empty;
                }

                if (selectRule.ParserType == CrawlConstants.PaserFieldName.ParserTypeRegex)
                {
                    if (selectRule.SelectMultiple)
                    {
                        var datas = new List<string>();
                        foreach (Match item in Regex.Matches(itemString, selectRule.SelectKey, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline))
                        {
                            if (item.Success)
                            {
                                if (item.Groups.Count >= 2)
                                {
                                    datas.Add(string.Join(", ", item.Groups.Skip(1).Select(q => q.Value)));
                                }
                                else
                                {
                                    datas.Add(item.Groups[0].Value);
                                }
                            }
                        }
                        if (datas.Count > 0)
                        {
                            setValue = string.Join("; ", datas);
                        }
                    }
                    else
                    {
                        var match = Regex.Match(itemString, selectRule.SelectKey, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
                        if (match.Success)
                        {
                            if (match.Groups.Count >= 2)
                            {
                                setValue = string.Join(", ", match.Groups.Skip(1).Select(q => q.Value));
                            }
                            else
                            {
                                setValue = match.Groups[0].Value;
                            }
                        }
                    }

                }
                if (selectRule.ParserType == CrawlConstants.PaserFieldName.ParserTypeXpath)
                {
                    HtmlDocument htmlDocument = new HtmlDocument
                    {
                        OptionAutoCloseOnEnd = true
                    };
                    htmlDocument.LoadHtml(itemString);
                    var htmlNodes = htmlDocument.DocumentNode.SelectNodes(selectRule.SelectKey);
                    if (htmlNodes?.Count > 0)
                    {
                        if (selectRule.SelectMultiple)
                        {
                            setValue = string.Join(", ", htmlNodes.Select(q =>
                            {
                                if (selectRule.IsHtml)
                                {
                                    return q.InnerHtml;
                                }
                                else
                                {
                                    return HttpUtility.HtmlDecode(q.InnerText);
                                }
                            }));
                        }
                        else
                        {
                            if (selectRule.IsHtml)
                            {
                                setValue = htmlNodes[0].InnerHtml;
                            }
                            else
                            {
                                setValue = htmlNodes[0].InnerText;
                            }
                        }
                    }
                }

                //Sau tất cả mã vẫn null và cấu hình dữ liệu mặc định không null thì thì set về dữ liệu mặc định
                if (string.IsNullOrEmpty(setValue) && !string.IsNullOrEmpty(selectRule.DefaultData))
                {
                    setValue = selectRule.DefaultData;
                }
            }

            return setValue;
        }

    }
}
