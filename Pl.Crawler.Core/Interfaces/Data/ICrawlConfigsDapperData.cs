using Pl.Crawler.Core.Entities;
using System.Collections.Generic;

namespace Pl.Crawler.Core.Interfaces
{
    public interface ICrawlConfigsDapperData
    {
        /// <summary>
        /// Get full website info
        /// </summary>
        /// <param name="id">Id website to get</param>
        /// <returns>Website</returns>
        Website GetWebsite(long id);

        /// <summary>
        /// Get list crawlConfig by web site Id
        /// </summary>
        /// <param name="websiteId">Id website</param>
        /// <returns>List CrawlConfig</returns>
        List<CrawlConfig> GetCrawlConfigsByWebsiteId(long websiteId);

        /// <summary>
        /// Get list parse field by crawl configuration id
        /// </summary>
        /// <param name="crawlConfigId">Id crawl configuation id</param>
        /// <returns>List PaserField</returns>
        List<ParseField> GetParseFieldsByCrawlConfigId(long crawlConfigId);

        /// <summary>
        /// Get list select rule by paser field id
        /// </summary>
        /// <param name="parseFieldId">The parser fild id to get them</param>
        /// <returns>List SelectRule</returns>
        List<SelectRule> GetSelectRulesByPaserFieldId(long parseFieldId);

        /// <summary>
        /// Get list replate rule by parse field id
        /// </summary>
        /// <param name="parseFieldId">Id parse field to get</param>
        /// <returns>List ReplateRule</returns>
        List<ReplateRule> GetReplateRulesByPaserFieldId(long parseFieldId);

        /// <summary>
        /// Get full infomation the crawl configuration
        /// </summary>
        /// <param name="configId">Id crawl configuration to get</param>
        /// <returns>CrawlConfig</returns>
        CrawlConfig GetCrawlConfigById(long configId);

    }
}
