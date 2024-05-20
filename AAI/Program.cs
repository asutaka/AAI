using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Quartz;
using AAI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AAI.Jobs;
using CoreTweet;

namespace AAI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        //if (!hostingContext.HostingEnvironment
                        //        .IsDevelopment())
                        config.AddJsonFile(
                            "/vault/secrets/appsettings.json",
                            optional: true, reloadOnChange: true);
                    })
                    .UseStartup<Startup>()
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Information);
                        logging.AddSentry(o =>
                        {
                            //o.Dsn = GlobalVariables.SentryDns;
                            o.MinimumEventLevel = LogLevel.Error;
                            o.MinimumBreadcrumbLevel = LogLevel.Error;
                            //o.Environment = GlobalVariables.SentryEnvironment;
                        });
                    })
                    .UseNLog(); 
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Add the required Quartz.NET services
                    services.AddQuartz(q =>
                    {
                        // Use a Scoped container to create jobs. I'll touch on this later
                        q.UseMicrosoftDependencyInjectionScopedJobFactory();
                        q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 1; });

                        // Register the job, loading the schedule from configuration
                        q.AddJobAndTrigger<TwitterJob>(hostContext.Configuration);
                        //q.AddJobAndTrigger<TwitterDetailJob>(hostContext.Configuration);
                    });

                    // Add the Quartz.NET hosted service
                    services.AddQuartzHostedService(
                        q => q.WaitForJobsToComplete = true);
                });
    }
}
