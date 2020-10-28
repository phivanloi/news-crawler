using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;

namespace Pl.Crawler.Data
{
    public class PaserFieldData : EfRepository<ParseField>, IPaserFieldData
    {
        public PaserFieldData(CrawlDbContext crawlDbContext) : base(crawlDbContext)
        {

        }


    }
}
