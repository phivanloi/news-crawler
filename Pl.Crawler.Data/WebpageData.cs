using Microsoft.Extensions.Options;
using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;
using Pl.Crawler.Core.Settings;
using System.Collections.Generic;
using MongoDB.Driver;
using System;
using System.Linq;

namespace Pl.Crawler.Data
{
    public class WebpageData : MogoRepository<Webpage>, IWebpageData
    {
        public WebpageData(IOptions<Connections> options) : base(options.Value.CrawlDataLog)
        {

        }

        /// <summary>
        /// Lấy top các bản ghi cần đưa vào queue
        /// </summary>
        /// <param name="top">Tốp bản ghi</param>
        /// <returns>List Webpage</returns>
        public List<Webpage> GetWebpagesToQueue(int top)
        {
            return Entitys.Find(q => q.NextDownloadTime <= DateTime.Now).Sort(Builders<Webpage>.Sort.Descending(q => q.DownloadRank)).Limit(top).ToList();
        }

        /// <summary>
        /// Hàm này update lần download tiếp theo cho webpage
        /// </summary>
        /// <param name="webpages">Danh sách webpage cần update</param>
        public void UpdateNextDownloadTime(List<Webpage> webpages)
        {
            var groupUpdate = webpages.GroupBy(q => q.DownloadRank).ToList();
            groupUpdate.ForEach(q =>
            {
                var createTime = DateTime.Now.AddDays(-30);
                var fistItem = q.FirstOrDefault();
                if (fistItem != null)
                {
                    createTime = fistItem.CreatedTime;
                }
                var nextTimeDownload = DateTime.Now.AddSeconds(GetTimeNextDownloadByRank(q.Key, createTime));
                var ids = q.Select(i => i.Id).ToList();
                var filter = Builders<Webpage>.Filter.In(f => f.Id, ids);
                var update = Builders<Webpage>.Update.Set(u => u.NextDownloadTime, nextTimeDownload);
                Entitys.UpdateMany(filter, update);
            });
        }

        /// <summary>
        /// Kiểm tra xem url đã có trong hệ thống hay chưa
        /// </summary>
        /// <param name="md5Url">Giá trị url</param>
        /// <returns></returns>
        public bool IsExistLink(string md5Url)
        {
            return Entitys.CountDocuments(q => q.UrlMd5 == md5Url) > 0;
        }

        /// <summary>
        /// Tạo index cho trường UrlMd5
        /// </summary>
        public void CreateIndexForUrlMd5()
        {
            Entitys.Indexes.CreateOne(new CreateIndexModel<Webpage>(Builders<Webpage>.IndexKeys.Ascending(q => q.UrlMd5)));
        }

        /// <summary>
        /// Tạo index cho trường NextDownloadTime
        /// </summary>
        public void CreateIndexForNextDownloadTime()
        {
            Entitys.Indexes.CreateOne(new CreateIndexModel<Webpage>(Builders<Webpage>.IndexKeys.Ascending(q => q.NextDownloadTime)));
        }

        /// <summary>
        /// Tạo index cho trường WebsiteId
        /// </summary>
        public void CreateIndexForWebsiteId()
        {
            Entitys.Indexes.CreateOne(new CreateIndexModel<Webpage>(Builders<Webpage>.IndexKeys.Descending(q => q.WebsiteId)));
        }

        /// <summary>
        /// Tạo index cho trường DownloadRank
        /// </summary>
        public void CreateIndexForDownloadRank()
        {
            Entitys.Indexes.CreateOne(new CreateIndexModel<Webpage>(Builders<Webpage>.IndexKeys.Descending(q => q.DownloadRank)));
        }

        /// <summary>
        /// lấy thời gian cho lần download tiếp theo theo rank
        /// </summary>
        /// <param name="rank">Page rank</param>
        /// <param name="crawlerTime">Crawler time</param>
        /// <returns></returns>
        public int GetTimeNextDownloadByRank(int rank, DateTime crawlerTime)
        {
            if (rank < 9 && crawlerTime <= DateTime.Now.AddDays(-30))
            {
                return 8640000;
            }

            if (rank < 9 && crawlerTime <= DateTime.Now.AddDays(-10))
            {
                return 864000;
            }

            switch (rank)
            {
                case 0:
                    return int.MaxValue;    //không xử lý lữa
                case 1:
                    return 864000000;       //10000 ngày
                case 2:
                    return 86400000;        //1000 ngày
                case 3:
                    return 8640000;         //100 ngày
                case 4:
                    return 86400;           //1 ngày
                case 5:
                    return 1800;            //30 phút
                case 6:
                    return 1500;            //25 phút
                case 7:
                    return 1200;            //20 phút
                case 8:
                    return 600;             //10 phút
                case 9:
                    return 300;             //5 phút
                case 10:
                    return 60;              //1 phút
                default:
                    return int.MaxValue;
            }
        }
    }
}
