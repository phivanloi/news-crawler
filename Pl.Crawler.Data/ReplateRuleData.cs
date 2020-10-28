using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;

namespace Pl.Crawler.Data
{
    public class ReplateRuleData : EfRepository<ReplateRule>, IReplateRuleData
    {
        public ReplateRuleData(CrawlDbContext crawlDbContext) : base(crawlDbContext)
        {

        }


    }
}
