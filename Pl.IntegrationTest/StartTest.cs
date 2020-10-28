using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pl.Crawelr.Caching;
using Pl.Crawler.Core.Interfaces;
using Pl.Crawler.Core.Services;
using Pl.Crawler.Core.Settings;
using Pl.Crawler.Data;
using Pl.Crawler.MessageQueue;
using Xunit;

namespace Pl.IntegrationTest
{
    public class StartTest
    {
        [Fact]
        public void CreateWebSiteAndConfig()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var testStartService = serviceProvider.GetService<CreateOrDeleteDataService>();
            var cacheService = serviceProvider.GetService<ICacheService>();

            testStartService.CreateData();
            cacheService.Clear();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            services.AddOptions();
            services.Configure<Connections>(configuration.GetSection("ConnectionStrings"));
            services.Configure<ConnectionMessageQueue>(configuration.GetSection("ConnectionMessageQueue"));

            var connections = configuration.GetSection("ConnectionStrings").Get<Connections>();
            services.AddDbContext<CrawlDbContext>(options => options.UseSqlServer(connections.CrawlConfiguration));
            CrawlDbContext crawlDbContext = services.BuildServiceProvider().GetService<CrawlDbContext>();
            crawlDbContext.Database.Migrate();

            services.AddDistributedRedisCache(options =>
            {
                var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>();
                options.Configuration = redisSettings.Configuration;
                options.InstanceName = redisSettings.InstanceName;
            });
            services.AddTransient<ICacheService, RedisCacheService>();

            services.AddTransient<IMesssageQueueService, RabbitmqService>();
            services.AddTransient<ICrawlConfigData, CrawlConfigData>();
            services.AddTransient<IPaserFieldData, PaserFieldData>();
            services.AddTransient<IReplateRuleData, ReplateRuleData>();
            services.AddTransient<ISelectRuleData, SelectRuleData>();
            services.AddTransient<IWebsiteData, WebsiteData>();
            services.AddTransient<IWebpageData, WebpageData>();
            services.AddTransient<IDataExportData, DataExportData>();
            services.AddTransient<ISitemapData, SitemapData>();
            services.AddTransient<IWebpageContentData, WebpageContentData>();
            services.AddTransient<ICacheCrawlConfigsService, CacheCrawlConfigsService>();
            services.AddTransient<ICrawlConfigsDapperData, CrawlConfigsDapperData>();

            services.AddTransient<FindlinkService>();
            services.AddTransient<ProcesserWebpageService>();
            services.AddTransient<PageDownloadService>();
            services.AddTransient<BeatService>();
            services.AddTransient<ExportService>();
            services.AddTransient<CreateOrDeleteDataService>();
        }
    }
}
