using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, logging) => {
                logging.AddApplicationInsights(hostingContext.Configuration.GetValue<string>("APPINSIGHTS_CONNECTIONSTRING"));
                logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information);
            }).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
