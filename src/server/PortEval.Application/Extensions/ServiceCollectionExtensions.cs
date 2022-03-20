﻿using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Services;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.FinancialDataFetcher;
using PortEval.Infrastructure;
using PortEval.Infrastructure.Repositories;
using System;
using System.Data;
using PortEval.Application.Services.BulkImportExport;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Application.Services.Queries;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Application.Services.Queries.TypeHandlers;
using PortEval.BackgroundJobs.DatabaseCleanup;
using PortEval.BackgroundJobs.InitialPriceFetch;
using PortEval.BackgroundJobs.LatestPricesFetch;
using PortEval.BackgroundJobs.MissingPricesFetch;

namespace PortEval.Application.Extensions
{
    /// <summary>
    /// Contains extension methods for PortEval's service configurations.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures PortEval's application services.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IInstrumentService, InstrumentService>();
            services.AddScoped<IPortfolioService, PortfolioService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IChartService, ChartService>();
            services.AddScoped<IInstrumentPriceService, InstrumentPriceService>();
            services.AddScoped<IDashboardService, DashboardService>();

            services.AddScoped<ICsvImportService, CsvImportService>();
            services.AddScoped<ICsvExportService, CsvExportService>();
            services.AddScoped<PortfolioImportProcessor>();
            services.AddScoped<PositionImportProcessor>();
            services.AddScoped<InstrumentImportProcessor>();
            services.AddScoped<TransactionImportProcessor>();
            services.AddScoped<PriceImportProcessor>();
        }

        /// <summary>
        /// Configures PortEval's repositories.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, PortEvalDbContext>();

            services.AddScoped<IInstrumentRepository, InstrumentRepository>();
            services.AddScoped<IInstrumentPriceRepository, InstrumentPriceRepository>();
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IChartRepository, ChartRepository>();
            services.AddScoped<IDashboardItemRepository, DashboardItemRepository>();
        }

        /// <summary>
        /// Configures PortEval's background jobs.
        /// </summary>
        /// <param name="services">ASP.NET services IoC container.</param>
        public static void AddBackgroundJobs(this IServiceCollection services)
        {
            services.AddScoped<IInitialPriceFetchJob, InitialPriceFetchJob>();
            services.AddScoped<ILatestPricesFetchJob, LatestPricesFetchJob>();
            services.AddScoped<ILatestExchangeRatesFetchJob, LatestExchangeRatesFetchJob>();
            services.AddScoped<IMissingExchangeRatesFetchJob, MissingExchangeRatesFetchJob>();
            services.AddScoped<IMissingInstrumentPricesFetchJob, MissingInstrumentPricesFetchJob>();
            services.AddScoped<IInstrumentPriceCleanupJob, InstrumentPriceCleanupJob>();
        }

        /// <summary>
        /// Configures PortEval's read queries.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void AddQueries(this IServiceCollection services)
        {
            services.AddScoped<IPortfolioQueries, PortfolioQueries>();
            services.AddScoped<IPositionQueries, PositionQueries>();
            services.AddScoped<ITransactionQueries, TransactionQueries>();
            services.AddScoped<IInstrumentQueries, InstrumentQueries>();
            services.AddScoped<ICurrencyQueries, CurrencyQueries>();
            services.AddScoped<ICurrencyExchangeRateQueries, CurrencyExchangeRateQueries>();
            services.AddScoped<IChartQueries, ChartQueries>();
            services.AddScoped<IDashboardLayoutQueries, DashboardLayoutQueries>();
        }

        /// <summary>
        /// Configures EF Core database context.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        /// <param name="configuration">ASP.NET application configuration.</param>
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PortEvalDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("PortEvalDb")));
        }

        /// <summary>
        /// Configures Dapper.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void ConfigureDapper(this IServiceCollection services)
        {
            services.AddScoped<IDbConnectionCreator, PortEvalDbConnection>();
            Dapper.SqlMapper.AddTypeHandler(new ColorHandler());
            Dapper.SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
        }

        /// <summary>
        /// Configures Hangfire.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        /// <param name="configuration">ASP.NET application configuration.</param>
        public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(h => h.UseSqlServerStorage(configuration.GetConnectionString("PortEvalDb"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                CommandTimeout = TimeSpan.FromMinutes(30),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));
            services.AddHangfireServer();
        }

        /// <summary>
        /// Configures internal instrument price and exchange rate fetching library.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        /// <param name="configuration">ASP.NET application configuration.</param>
        public static void ConfigurePriceFetcher(this IServiceCollection services, IConfiguration configuration)
        {
            var fetcher = new PriceFetcher();
            var tiingoKey = Environment.GetEnvironmentVariable("PORTEVAL_Tiingo_Key");
            var openExchangeRatesKey = Environment.GetEnvironmentVariable("PORTEVAL_OpenExchangeRates_Key");
            if (tiingoKey != null)
            {
                fetcher.AddTiingo(tiingoKey, new RateLimiter(TimeSpan.FromHours(1), 500));
            }

            if (openExchangeRatesKey != null)
            {
                fetcher.AddOpenExchangeRates(openExchangeRatesKey, new RateLimiter(TimeSpan.FromHours(3), 4));
            }

            fetcher.AddExchangeRateHost();

            services.AddSingleton(fetcher);
        }
    }
}
