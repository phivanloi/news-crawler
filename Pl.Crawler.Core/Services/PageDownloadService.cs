using Pl.Crawler.Core.Interfaces;
using System;
using System.Text;
using Pl.Crawler.Core.Constants;
using System.Diagnostics;
using System.Net;
using System.IO;
using Pl.Crawler.Core.Entities;

namespace Pl.Crawler.Core.Services
{
    public class PageDownloadService : IServiceStart
    {
        private readonly IWebpageData webpageData;
        private readonly IWebpageContentData webpageContentData;
        private readonly IMesssageQueueService messsageQueueService;
        private readonly ICacheCrawlConfigsService cacheGetCrawlConfigService;

        public PageDownloadService(
            ICacheCrawlConfigsService _cacheGetCrawlConfigService,
            IWebpageContentData _webpageContentData,
            IMesssageQueueService _messsageQueueService,
            IWebpageData _webpageData)
        {
            webpageData = _webpageData;
            messsageQueueService = _messsageQueueService;
            webpageContentData = _webpageContentData;
            cacheGetCrawlConfigService = _cacheGetCrawlConfigService;
        }

        public void StartService()
        {
            messsageQueueService.RegisterConsoleWorker(MessageQueueConstants.DownloadWebContentTaskQueueName, (message) =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var webpage = webpageData.FindByKey(message);
                if (webpage == null)
                {
                    return false;
                }

                var checkDownload = DownloadContent(webpage);
                if (checkDownload)
                {
                    var website = cacheGetCrawlConfigService.GetWebsite(webpage.WebsiteId);
                    if (website.FindLinkOnlySiteMap)
                    {
                        if (webpage.IsSiteMapPage)
                        {
                            messsageQueueService.PublishTask(MessageQueueConstants.FindlinkWebContentTaskQueueName, message);
                        }
                    }
                    else
                    {
                        messsageQueueService.PublishTask(MessageQueueConstants.FindlinkWebContentTaskQueueName, message);
                    }
                    messsageQueueService.PublishTask(MessageQueueConstants.ProcessWebContentTaskQueueName, message);
                }

                stopWatch.Stop();
                $"Download page Id {message} in {stopWatch.ElapsedMilliseconds} miniseconds".WriteConsole(ConsoleColor.Yellow);
                return true;
            });
        }

        /// <summary>
        /// Download web page content
        /// </summary>
        /// <param name="webpage">WWebpage to download</param>
        /// <returns>bool</returns>
        public bool DownloadContent(Webpage webpage)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webpage.Url);
                request.UserAgent = CommonConstants.RequestUserAgent;
                request.AllowAutoRedirect = true;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                webpage.LastStatusCode = response.StatusCode.ToString();
                $"Download webpage url: {webpage.Url} status code {response.StatusCode} ".WriteConsole(ConsoleColor.Blue);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                        var webContent = readStream.ReadToEnd();
                        readStream.Close();
                        response.Close();
                        webpage.ErrorDownloadCount = 0;
                        webpage.LastDownloaded = DateTime.Now;
                        webpage.NextDownloadTime = DateTime.Now.AddSeconds(webpageData.GetTimeNextDownloadByRank(webpage.DownloadRank, webpage.CreatedTime));//Tại đây vấn sét lại next time download để trừ đi thời gian đợi download
                        webpageContentData.UpdateOrCreateContent(webpage.Id, webContent);
                        return webpageData.Update(webpage);
                    default:
                        SetNexTimeDownloadOnError(ref webpage);
                        return false;
                }

            }
            catch (Exception ex)
            {
                $"Error download webpage url: {webpage.Url}".WriteConsole(ConsoleColor.Red);
                ex.Message.WriteConsole(ConsoleColor.Red);
                SetNexTimeDownloadOnError(ref webpage);
                return false;
            }
        }

        /// <summary>
        /// Hàm tính toán tụt rank nếu download lỗi và thực hiện đặt lại thời gian download cho lần tiếp theo
        /// </summary>
        /// <param name="webpage">Webpage cần download</param>
        public void SetNexTimeDownloadOnError(ref Webpage webpage)
        {
            webpage.ErrorDownloadCount += 1;
            if (webpage.ErrorDownloadCount >= CrawlConstants.MaxErrorDownloadCount && webpage.DownloadRank < 9)//những page có rank từ 9 trở lên sẽ không bao giờ bị tụt rank
            {
                webpage.DownloadRank = webpage.DownloadRank > 0 ? webpage.DownloadRank - 1 : 0;
                webpage.ErrorDownloadCount = 0;
            }
            webpage.NextDownloadTime = DateTime.Now.AddSeconds(webpageData.GetTimeNextDownloadByRank(webpage.DownloadRank, webpage.CreatedTime));//Tại đây vấn sét lại next time download để trừ đi thời gian đợi download
            webpageData.Update(webpage);
        }
    }
}
