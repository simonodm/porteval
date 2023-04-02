using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PortEval.Infrastructure;
using System;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using Serilog;

namespace PortEval.Application
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<PortEvalDbContext>();
                if (dbContext.Database.IsSqlServer())
                {
                    dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                throw;
            }

            try
            {
                ScheduleBackgroundJobs(scope);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while scheduling background jobs.");

                throw;
            }

            try
            {
                host.Run();
            }
            catch
            {
                await host.StopAsync();
                Environment.Exit(-1);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void ScheduleBackgroundJobs(IServiceScope scope)
        {
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<ILatestPricesFetchJob>("latest_prices", job => job.Run(), "*/5 * * * *");
            recurringJobManager.AddOrUpdate<IMissingInstrumentPricesFetchJob>("fetch_missing_prices", job => job.Run(), Cron.Daily);
            recurringJobManager.AddOrUpdate<IMissingExchangeRatesFetchJob>("fetch_missing_exchange_rates",
                job => job.Run(), Cron.Daily);
            recurringJobManager.AddOrUpdate<IInstrumentPriceCleanupJob>("db_cleanup", job => job.Run(), Cron.Daily);
            recurringJobManager.AddOrUpdate<IImportCleanupJob>("import_cleanup", job => job.Run(), Cron.Daily);
            recurringJobManager.AddOrUpdate<ISplitFetchJob>("split_fetch", job => job.Run(), Cron.Daily);
            recurringJobManager.AddOrUpdate<ISplitPriceAndTransactionAdjustmentJob>("split_price_adjustment", job => job.Run(), Cron.Daily);

            recurringJobManager.Trigger("db_cleanup");
            recurringJobManager.Trigger("fetch_missing_prices");
            recurringJobManager.Trigger("fetch_missing_exchange_rates");
            recurringJobManager.Trigger("import_cleanup");
            recurringJobManager.Trigger("split_price_adjustment");
            recurringJobManager.Trigger("split_fetch");
        }
    }
}
