using Dapper;
using Microsoft.Extensions.Options;
using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;
using Pl.Crawler.Core.Settings;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Pl.Crawler.Data
{
    public class CrawlConfigsDapperData : ICrawlConfigsDapperData
    {
        private readonly Connections connections;
        public CrawlConfigsDapperData(IOptions<Connections> options)
        {
            connections = options.Value;
        }

        /// <summary>
        /// Get full website info
        /// </summary>
        /// <param name="id">Id website to get</param>
        /// <returns>Website</returns>
        public Website GetWebsite(long id)
        {
            using (SqlConnection connection = new SqlConnection(connections.CrawlConfiguration))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Website>($"SELECT TOP(1) * FROM Websites WHERE Id = {id}");
            }
        }

        /// <summary>
        /// Get list crawlConfig by web site Id
        /// </summary>
        /// <param name="websiteId">Id website</param>
        /// <returns>List CrawlConfig</returns>
        public List<CrawlConfig> GetCrawlConfigsByWebsiteId(long websiteId)
        {
            using (SqlConnection connection = new SqlConnection(connections.CrawlConfiguration))
            {
                connection.Open();
                return connection.Query<CrawlConfig>($"SELECT * FROM CrawlConfigs WHERE WebsiteId = {websiteId}").AsList();
            }
        }

        /// <summary>
        /// Get list parse field by crawl configuration id
        /// </summary>
        /// <param name="crawlConfigId">Id crawl configuation id</param>
        /// <returns>List PaserField</returns>
        public List<ParseField> GetParseFieldsByCrawlConfigId(long crawlConfigId)
        {
            using (SqlConnection connection = new SqlConnection(connections.CrawlConfiguration))
            {
                connection.Open();
                return connection.Query<ParseField>($"SELECT * FROM ParseFields WHERE CrawlConfigId = {crawlConfigId}").AsList();
            }
        }

        /// <summary>
        /// Get list select rule by paser field id
        /// </summary>
        /// <param name="parseFieldId">The parser fild id to get them</param>
        /// <returns>List SelectRule</returns>
        public List<SelectRule> GetSelectRulesByPaserFieldId(long parseFieldId)
        {
            using (SqlConnection connection = new SqlConnection(connections.CrawlConfiguration))
            {
                connection.Open();
                return connection.Query<SelectRule>($"SELECT * FROM SelectRules WHERE ParseFieldId = {parseFieldId}").AsList();
            }
        }

        /// <summary>
        /// Get list replate rule by parse field id
        /// </summary>
        /// <param name="parseFieldId">Id parse field to get</param>
        /// <returns>List ReplateRule</returns>
        public List<ReplateRule> GetReplateRulesByPaserFieldId(long parseFieldId)
        {
            using (SqlConnection connection = new SqlConnection(connections.CrawlConfiguration))
            {
                connection.Open();
                return connection.Query<ReplateRule>($"SELECT * FROM ReplateRules WHERE ParseFieldId = {parseFieldId}").AsList();
            }
        }

        /// <summary>
        /// Get full infomation the crawl configuration
        /// </summary>
        /// <param name="configId">Id crawl configuration to get</param>
        /// <returns>CrawlConfig</returns>
        public CrawlConfig GetCrawlConfigById(long configId)
        {
            using (SqlConnection connection = new SqlConnection(connections.CrawlConfiguration))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<CrawlConfig>($"SELECT TOP(1) * FROM CrawlConfigs WHERE Id = {configId}");
            }
        }

    }
}
