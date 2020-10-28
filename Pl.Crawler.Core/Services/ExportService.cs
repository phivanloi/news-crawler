using Pl.Crawler.Core.Interfaces;
using System;
using System.Collections.Generic;
using Pl.Crawler.Core.Constants;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Diagnostics;

namespace Pl.Crawler.Core.Services
{
    public class ExportService : IServiceStart
    {
        private readonly IWebpageData webpageData;
        private readonly IDataExportData dataExportData;
        private readonly IMesssageQueueService messsageQueueService;
        private readonly ICacheCrawlConfigsService cacheGetCrawlConfigService;

        public ExportService(
            ICacheCrawlConfigsService _cacheGetCrawlConfigService,
            IDataExportData _dataExportData,
            IMesssageQueueService _messsageQueueService,
            IWebpageData _webpageData)
        {
            webpageData = _webpageData;
            messsageQueueService = _messsageQueueService;
            dataExportData = _dataExportData;
            cacheGetCrawlConfigService = _cacheGetCrawlConfigService;
        }

        public void StartService()
        {
            messsageQueueService.RegisterConsoleWorker(MessageQueueConstants.DataExportTaskQueueName, (message) =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var idExport = SqlDexport(message);
                if (idExport)
                {
                    messsageQueueService.PublishTask(MessageQueueConstants.NotificationTaskQueueName, message);
                }

                stopWatch.Stop();
                $"Export page Id {message} in {stopWatch.ElapsedMilliseconds} miniseconds".WriteConsole(ConsoleColor.Yellow);
                return true;
            });
        }

        public bool ApiExport(string dataExportId)
        {
            var dataExport = dataExportData.FindByKey(dataExportId);
            if (dataExport == null)
            {
                return false;
            }

            try
            {
                $"Start Export data Id: {dataExportId}".WriteConsole(ConsoleColor.Blue);

                var crawlConfigs = cacheGetCrawlConfigService.GetCrawlConfigById(dataExport.CrawlConfigId);
                var webpage = webpageData.FindByKey(dataExport.WebpageId);

                if (crawlConfigs == null || webpage == null)
                {
                    return false;
                }

                var paserData = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataExport.PaserData);
                Uri uri = new Uri(webpage.Url);
                var apiSecurityCode = Utility.GetQueryString(null, "ApiSecurityCode", crawlConfigs.ExportApiUrl);
                var sendObject = new
                {
                    apiSecurityCode,
                    files = new List<object>(),
                    domain = $"{uri.Scheme}://{uri.Host}",
                    news = new
                    {
                        id = webpage.UrlMd5,
                        categoryName = paserData.GetValueOrDefault("CategoryName"),
                        categoryID = 0,
                        title = paserData.GetValueOrDefault("Title"),
                        summary = paserData.GetValueOrDefault("Description"),
                        contentNew = paserData.GetValueOrDefault("Content"),
                        imageUrl = paserData.GetValueOrDefault("Image"),
                        tag = paserData.GetValueOrDefault("TagKeywords"),
                        author = "Crawler",
                        source = "",
                        readTime = 0,
                        isDelete = false,
                        dateCreate = dataExport.CreatedTime,
                        datePublic = ConvetPublishDate(paserData.GetValueOrDefault("PublishTime")),
                        ishot = false
                    }
                };
                string result = Utility.CallRestfulUrlAsync(Utility.RemoveQueryString(crawlConfigs.ExportApiUrl, "ApiSecurityCode"), HttpMethods.Post, JsonConvert.SerializeObject(sendObject)).Result;
                return true;
            }
            catch (Exception ex)
            {
                $"Error Export data Id: {dataExportId}".WriteConsole(ConsoleColor.Yellow);
                ex.Message.WriteConsole(ConsoleColor.Red);
                return false;
            }
            finally
            {
                dataExport.IsNew = false;
                dataExport.IsUpdate = false;
                dataExport.LastExportTime = DateTime.Now;
                dataExportData.Update(dataExport);
            }
        }

        public bool SqlDexport(string dataExportId)
        {
            var dataExport = dataExportData.FindByKey(dataExportId);
            if (dataExport == null)
            {
                return false;
            }

            try
            {
                $"Start Export data Id: {dataExportId}".WriteConsole(ConsoleColor.Blue);

                var crawlConfigs = cacheGetCrawlConfigService.GetCrawlConfigById(dataExport.CrawlConfigId);
                var webpage = webpageData.FindByKey(dataExport.WebpageId);

                if (crawlConfigs == null || webpage == null)
                {
                    return false;
                }

                var paserData = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataExport.PaserData);
                Uri uri = new Uri(webpage.Url);
                var apiSecurityCode = Utility.GetQueryString(null, "ApiSecurityCode", crawlConfigs.ExportApiUrl);
                var sendObject = new
                {
                    apiSecurityCode,
                    files = new List<object>(),
                    domain = $"{uri.Scheme}://{uri.Host}",
                    news = new
                    {
                        id = webpage.UrlMd5,
                        categoryName = paserData.GetValueOrDefault("CategoryName"),
                        categoryID = 0,
                        title = paserData.GetValueOrDefault("Title"),
                        summary = paserData.GetValueOrDefault("Description"),
                        contentNew = paserData.GetValueOrDefault("Content"),
                        imageUrl = paserData.GetValueOrDefault("Image"),
                        tag = paserData.GetValueOrDefault("TagKeywords"),
                        author = "Crawler",
                        source = "",
                        readTime = 0,
                        isDelete = false,
                        dateCreate = dataExport.CreatedTime,
                        datePublic = ConvetPublishDate(paserData.GetValueOrDefault("PublishTime")),
                        ishot = false
                    }
                };
                string result = Utility.CallRestfulUrlAsync(Utility.RemoveQueryString(crawlConfigs.ExportApiUrl, "ApiSecurityCode"), HttpMethods.Post, JsonConvert.SerializeObject(sendObject)).Result;
                return true;
            }
            catch (Exception ex)
            {
                $"Error Export data Id: {dataExportId}".WriteConsole(ConsoleColor.Yellow);
                ex.Message.WriteConsole(ConsoleColor.Red);
                return false;
            }
            finally
            {
                dataExport.IsNew = false;
                dataExport.IsUpdate = false;
                dataExport.LastExportTime = DateTime.Now;
                dataExportData.Update(dataExport);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="convetData">Chuỗi cần paser</param>
        /// <returns></returns>
        private DateTime ConvetPublishDate(string convetData)
        {
            if (string.IsNullOrEmpty(convetData))
            {
                return DateTime.Now;
            }

            CultureInfo enUS = new CultureInfo("en-US");

            if (DateTime.TryParseExact(convetData, "dd/MM/yyyy hh:mm:ss", enUS, DateTimeStyles.None, out DateTime dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "dd/MM/yyyy hh:mm", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "dd/MM/yyyy hh:", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "dd/MM/yyyy", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "MM/dd/yyyy hh:mm:ss", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "MM/dd/yyyy hh:mm", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "MM/dd/yyyy hh", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "MM/dd/yyyy", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "dd:MM:yyyy hh:mm:ss", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "dd:MM:yyyy hh:mm", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "dd:MM:yyyy hh:", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "dd:MM:yyyy", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "MM:dd:yyyy hh:mm:ss", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "MM:dd:yyyy hh:mm", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "MM:dd:yyyy hh", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            if (DateTime.TryParseExact(convetData, "MM:dd:yyyy", enUS, DateTimeStyles.None, out dateValue))
                return dateValue;

            return DateTime.Now;
        }
    }
}
