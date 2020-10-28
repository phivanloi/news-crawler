using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;

namespace Pl.Crawler.Data
{
    public class CrawlConfigData : EfRepository<CrawlConfig>, ICrawlConfigData
    {
        public CrawlConfigData(CrawlDbContext crawlDbContext) : base(crawlDbContext)
        {

        }


    }
}
