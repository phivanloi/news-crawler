using Pl.Crawler.Core.Interfaces;
using System;
using Pl.Crawler.Core.Constants;
using System.Diagnostics;
using System.Threading;

namespace Pl.Crawler.Core.Services
{
    public class BeatService : IServiceStart
    {
        private readonly IWebpageData webpageData;
        private readonly IMesssageQueueService messsageQueueService;

        public BeatService(
            IMesssageQueueService _messsageQueueService,
            IWebpageData _webpageData)
        {
            webpageData = _webpageData;
            messsageQueueService = _messsageQueueService;
        }

        public void StartService()
        {
            while (true)
            {
                try
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    var webpages = webpageData.GetWebpagesToQueue(10);
                    if (0 < webpages?.Count)
                    {
                        foreach (var webpage in webpages)
                        {
                            messsageQueueService.PublishTask(MessageQueueConstants.DownloadWebContentTaskQueueName, webpage.Id);
                        }
                        webpageData.UpdateNextDownloadTime(webpages);
                    }
                    else
                    {
                        Thread.Sleep(3000);
                    }

                    stopWatch.Stop();
                    $"Beater select and send {webpages?.Count ?? 0} row in {stopWatch.ElapsedMilliseconds} miniseconds".WriteConsole(ConsoleColor.Yellow);
                }
                catch (Exception ex)
                {
                    ex.ToString().WriteConsole(ConsoleColor.Red);
                }
            }
        }
    }
}
