using Pl.Crawler.Core.Entities;
using System.Collections.Generic;

namespace Pl.Crawler.Core.Interfaces
{
    public interface ICacheCrawlConfigsService
    {
        /// <summary>
        /// Lấy thông tin website từ cache
        /// </summary>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        Website GetWebsite(long websiteId);

        /// <summary>
        /// Lấy danh sách cấu hình bằng websiteid
        /// </summary>
        /// <param name="websiteId">Id website</param>
        /// <returns>List CrawlConfig</returns>
        List<CrawlConfig> GetCrawlConfigsByWebsiteId(long websiteId);

        /// <summary>
        /// Lấy danh sách phần tích trường theo id cấu hình
        /// </summary>
        /// <param name="crawlConfigId">Id cấu hình</param>
        /// <returns>List PaserField</returns>
        List<ParseField> GetParseFieldsByCrawlConfigId(long crawlConfigId);

        /// <summary>
        /// Lấy danh sách quy tắc lựa chọn dựa vào id phân tích trường
        /// </summary>
        /// <param name="paserFieldId">Id cấu hình</param>
        /// <returns>List SelectRule</returns>
        List<SelectRule> GetSelectRulesByPaserFieldId(long paserFieldId);

        /// <summary>
        /// Lấy danh sách quy tắc thay thế dữ liệu dựa vào phần tích trường
        /// </summary>
        /// <param name="paserFieldId">Id cấu hình</param>
        /// <returns>List ReplateRule</returns>
        List<ReplateRule> GetReplateRulesByPaserFieldId(long paserFieldId);

        /// <summary>
        /// Lấy thông tin cấu hình theo id
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        CrawlConfig GetCrawlConfigById(long configId);

    }
}
