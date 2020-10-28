using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pl.Crawler.Core;
using Pl.Crawler.Core.Interfaces;
using Pl.Crawler.Core.Services;
using Pl.Crawler.Core.Settings;
using Pl.Crawler.Data;
using Pl.Crawler.MessageQueue;

namespace Pl.Crawler.Beater
{
    class Program
    {
        static void Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            "Pl.Crawler.Beater started".WriteConsole(ConsoleColor.Blue);
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

            services.AddTransient<IMesssageQueueService, RabbitmqService>();
            services.AddTransient<IWebpageData, WebpageData>();
            services.AddTransient<IServiceStart, BeatService>();
        }
    }
}