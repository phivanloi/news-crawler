using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;

namespace Pl.Crawler.Data
{
    public class SelectRuleData : EfRepository<SelectRule>, ISelectRuleData
    {
        public SelectRuleData(CrawlDbContext crawlDbContext) : base(crawlDbContext)
        {

        }


    }
}
