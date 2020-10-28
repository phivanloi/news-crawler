using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;

namespace Pl.Crawler.Data
{
    public class SitemapData : EfRepository<Sitemap>, ISitemapData
    {
        public SitemapData(CrawlDbContext crawlDbContext) : base(crawlDbContext)
        {

        }


    }
}
