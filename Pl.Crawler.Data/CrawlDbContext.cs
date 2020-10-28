using Microsoft.EntityFrameworkCore;
using Pl.Crawler.Core.Entities;

namespace Pl.Crawler.Data
{
    public class CrawlDbContext : DbContext
    {
        public CrawlDbContext(DbContextOptions<CrawlDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Website> Websites { get; set; }
        public virtual DbSet<CrawlConfig> CrawlConfigs { get; set; }
        public virtual DbSet<ParseField> ParseFields { get; set; }
        public virtual DbSet<SelectRule> SelectRules { get; set; }
        public virtual DbSet<ReplateRule> ReplateRules { get; set; }
        public virtual DbSet<Sitemap> Sitemaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Website>()
                .HasIndex(n => n.Domain);
        }
    }
}