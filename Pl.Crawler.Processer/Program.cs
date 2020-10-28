using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pl.Crawelr.Caching;
using Pl.Crawler.Core;
using Pl.Crawler.Core.Interfaces;
using Pl.Crawler.Core.Services;
using Pl.Crawler.Core.Settings;
using Pl.Crawler.Data;
using Pl.Crawler.MessageQueue;

namespace Pl.Crawler.Processer
{
    class Program
    {
        static void Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            "Pl.Crawler.Processer started".WriteConsole(ConsoleColor.Blue);
            serviceProvider.GetService<IServiceStart>().StartService();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder
                .AddDebug()
                .AddConsole()
            );

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            services.AddOptions();
            services.Configure<Connections>(configuration.GetSection("ConnectionStrings"));
            services.Configure<ConnectionMessageQueue>(configuration.GetSection("ConnectionMessageQueue"));

            services.AddDistributedRedisCache(options =>
            {
                var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>();
                options.Configuration = redisSettings.Configuration;
                options.InstanceName = redisSettings.InstanceName;
            });
            services.AddTransient<ICacheService, RedisCacheService>();

            services.AddTransient<IMesssageQueueService, RabbitmqService>();
            services.AddTransient<IWebpageData, WebpageData>();
            services.AddTransient<IWebpageContentData, WebpageContentData>();
            services.AddTransient<IDataExportData, DataExportData>();

            services.AddTransient<ICrawlConfigsDapperData, CrawlConfigsDapperData>();
            services.AddTransient<ICacheCrawlConfigsService, CacheCrawlConfigsService>();
            services.AddTransient<IServiceStart, ProcesserWebpageService>();
        }
    }
}
