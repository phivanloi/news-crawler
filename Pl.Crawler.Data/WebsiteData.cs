using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;

namespace Pl.Crawler.Data
{
    public class WebsiteData : EfRepository<Website>, IWebsiteData
    {
        public WebsiteData(CrawlDbContext crawlDbContext) : base(crawlDbContext)
        {

        }


    }
}
