using Pl.Crawler.Core.Entities;
using System;
using System.Collections.Generic;

namespace Pl.Crawler.Core.Interfaces
{
    public interface IWebpageData : IRepository<Webpage>
    {
        /// <summary>
        /// Lấy top các bản ghi cần đưa vào queue
        /// </summary>
        /// <param name="top">Tốp bản ghi</param>
        /// <returns>List Webpage</returns>
        List<Webpage> GetWebpagesToQueue(int top);

        /// <summary>
        /// Hàm này update lần download tiếp theo cho webpage
        /// </summary>
        /// <param name="webpages">Danh sách webpage cần update</param>
        void UpdateNextDownloadTime(List<Webpage> webpages);

        /// <summary>
        /// Kiểm tra xem url đã có trong hệ thống hay chưa
        /// </summary>
        /// <param name="md5Url">Giá trị url</param>
        /// <returns></returns>
        bool IsExistLink(string md5Url);

        /// <summary>
        /// Tạo index cho trường UrlMd5
        /// </summary>
        void CreateIndexForUrlMd5();

        /// <summary>
        /// Tạo index cho trường NextDownloadTime
        /// </summary>
        void CreateIndexForNextDownloadTime();

        /// <summary>
        /// Tạo index cho trường WebsiteId
        /// </summary>
        void CreateIndexForWebsiteId();

        /// <summary>
        /// Tạo index cho trường DownloadRank
        /// </summary>
        void CreateIndexForDownloadRank();

        /// <summary>
        /// lấy thời gian cho lần download tiếp theo theo rank
        /// </summary>
        /// <param name="rank">Page rank</param>
        /// <param name="crawlerTime">Crawler time</param>
        /// <returns></returns>
        int GetTimeNextDownloadByRank(int rank, DateTime crawlerTime);
    }
}
