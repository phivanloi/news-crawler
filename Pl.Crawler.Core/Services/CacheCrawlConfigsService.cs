using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;
using System.Collections.Generic;

namespace Pl.Crawler.Core.Services
{
    public class CacheCrawlConfigsService : ICacheCrawlConfigsService
    {
        private readonly ICrawlConfigsDapperData crawlConfigsDapperData;
        private readonly ICacheService cacheService;

        public CacheCrawlConfigsService(
            ICrawlConfigsDapperData _crawlConfigsDapperData,
            ICacheService _cacheService)
        {
            cacheService = _cacheService;
            crawlConfigsDapperData = _crawlConfigsDapperData;
        }

        /// <summary>
        /// Lấy thông tin website từ cache
        /// </summary>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        public Website GetWebsite(long websiteId)
        {
            var cacheKey = $"cache_website_{websiteId}";
            return cacheService.GetOrCreate(cacheKey, () =>
            {
                return crawlConfigsDapperData.GetWebsite(websiteId);
            }, 3600);
        }

        /// <summary>
        /// Lấy danh sách cấu hình bằng websiteid
        /// </summary>
        /// <param name="websiteId">Id website</param>
        /// <returns>List CrawlConfig</returns>
        public List<CrawlConfig> GetCrawlConfigsByWebsiteId(long websiteId)
        {
            var key = $"GetCrawlConfigsByWebsiteId_{websiteId}";
            return cacheService.GetOrCreate(key, () =>
            {
                return crawlConfigsDapperData.GetCrawlConfigsByWebsiteId(websiteId);
            }, 3600);
        }

        /// <summary>
        /// Lấy danh sách phần tích trường theo id cấu hình
        /// </summary>
        /// <param name="crawlConfigId">Id cấu hình</param>
        /// <returns>List PaserField</returns>
        public List<ParseField> GetParseFieldsByCrawlConfigId(long crawlConfigId)
        {
            var key = $"GetPaserFieldsByWebsiteId_{crawlConfigId}";
            return cacheService.GetOrCreate(key, () =>
            {
                return crawlConfigsDapperData.GetParseFieldsByCrawlConfigId(crawlConfigId);
            }, 3600);
        }

        /// <summary>
        /// Lấy danh sách quy tắc lựa chọn dựa vào id phân tích trường
        /// </summary>
        /// <param name="paserFieldId">Id cấu hình</param>
        /// <returns>List SelectRule</returns>
        public List<SelectRule> GetSelectRulesByPaserFieldId(long paserFieldId)
        {
            var key = $"GetSelectRulesByWebsiteId_{paserFieldId}";
            return cacheService.GetOrCreate(key, () =>
            {
                return crawlConfigsDapperData.GetSelectRulesByPaserFieldId(paserFieldId);
            }, 3600);
        }

        /// <summary>
        /// Lấy danh sách quy tắc thay thế dữ liệu dựa vào phần tích trường
        /// </summary>
        /// <param name="paserFieldId">Id cấu hình</param>
        /// <returns>List ReplateRule</returns>
        public List<ReplateRule> GetReplateRulesByPaserFieldId(long paserFieldId)
        {
            var key = $"GetReplateRulesByPaserFieldId_{paserFieldId}";
            return cacheService.GetOrCreate(key, () =>
            {
                return crawlConfigsDapperData.GetReplateRulesByPaserFieldId(paserFieldId);
            }, 3600);
        }

        /// <summary>
        /// Lấy thông tin cấu hình theo id
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public CrawlConfig GetCrawlConfigById(long configId)
        {
            var key = $"GetCrawlConfigById_{configId}";
            return cacheService.GetOrCreate(key, () =>
            {
                return crawlConfigsDapperData.GetCrawlConfigById(configId);
            }, 3600);
        }

    }
}
