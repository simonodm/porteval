using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.Common.Calculators;
using PortEval.Application.Features.Common.ChartDataGenerators;
using PortEval.Application.Features.Interfaces;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Features.Queries;
using PortEval.Application.Features.Queries.TypeHandlers;
using PortEval.Application.Features.Services;
using PortEval.Application.Features.Services.BulkImportExport;
using PortEval.Application.Models.DTOs;
using PortEval.BackgroundJobs;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Interfaces;
using PortEval.Domain.Services;
using PortEval.Infrastructure;
using PortEval.Infrastructure.FinancialDataFetcher.ExchangeRateHost;
using PortEval.Infrastructure.FinancialDataFetcher.OpenExchangeRates;
using PortEval.Infrastructure.FinancialDataFetcher.RapidAPIMboum;
using PortEval.Infrastructure.FinancialDataFetcher.Tiingo;
using PortEval.Infrastructure.Repositories;
using System;
using System.Data;
using System.IO.Abstractions;

namespace PortEval.Application.Extensions
{
    /// <summary>
    /// Contains extension methods for PortEval's service configurations.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Injects PortEval's application services.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IInstrumentService, InstrumentService>();
            services.AddScoped<IPortfolioService, PortfolioService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IChartService, ChartService>();
            services.AddScoped<IInstrumentPriceService, InstrumentPriceService>();
            services.AddScoped<IInstrumentSplitService, InstrumentSplitService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<INotificationService, NotificationService>();

            var fileStoragePath = configuration.GetConfigurationValue("PORTEVAL_File_Storage");
            services.AddScoped<IFileSystem, FileSystem>();
            services.AddScoped<ICsvImportService, CsvImportService>(
                provider =>
                    new CsvImportService(provider.GetRequiredService<IDataImportRepository>(), provider.GetRequiredService<IBackgroundJobClient>(), provider.GetRequiredService<IFileSystem>(), fileStoragePath)
            );
            services.AddScoped<ICsvExportService, CsvExportService>();

            services.AddScoped<IImportProcessor<PortfolioDto>, PortfolioImportProcessor>();
            services.AddScoped<IImportProcessor<PositionDto>, PositionImportProcessor>();
            services.AddScoped<IImportProcessor<InstrumentDto>, InstrumentImportProcessor>();
            services.AddScoped<IImportProcessor<TransactionDto>, TransactionImportProcessor>();
            services.AddScoped<IImportProcessor<InstrumentPriceDto>, PriceImportProcessor>();

            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
        }

        /// <summary>
        /// Injects PortEval's repositories.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, PortEvalDbContext>();

            services.AddScoped<IInstrumentRepository, InstrumentRepository>();
            services.AddScoped<IInstrumentPriceRepository, InstrumentPriceRepository>();
            services.AddScoped<IInstrumentSplitRepository, InstrumentSplitRepository>();
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ICurrencyExchangeRateRepository, CurrencyExchangeRateRepository>();
            services.AddScoped<IChartRepository, ChartRepository>();
            services.AddScoped<IDashboardItemRepository, DashboardItemRepository>();
            services.AddScoped<IDataImportRepository, DataImportRepository>();
            services.AddScoped<IExchangeRepository, ExchangeRepository>();
        }

        /// <summary>
        /// Injects PortEval's background jobs.
        /// </summary>
        /// <param name="services">ASP.NET services IoC container.</param>
        public static void AddBackgroundJobs(this IServiceCollection services)
        {
            services.AddScoped<IInitialPriceFetchJob, InitialPriceFetchJob>();
            services.AddScoped<ILatestPricesFetchJob, LatestPricesFetchJob>();
            services.AddScoped<IMissingExchangeRatesFetchJob, MissingExchangeRatesFetchJob>();
            services.AddScoped<IMissingInstrumentPricesFetchJob, MissingInstrumentPricesFetchJob>();
            services.AddScoped<IInstrumentPriceCleanupJob, InstrumentPriceCleanupJob>();
            services.AddScoped<IDataImportJob, DataImportJob>();
            services.AddScoped<IImportCleanupJob, ImportCleanupJob>();
            services.AddScoped<ISplitPriceAndTransactionAdjustmentJob, SplitPriceAndTransactionAdjustmentJob>();
            services.AddScoped<ISplitFetchJob, SplitFetchJob>();
        }

        /// <summary>
        /// Injects PortEval's read queries.
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
            services.AddScoped<IDataImportQueries, DataImportQueries>();
            services.AddScoped<IExchangeQueries, ExchangeQueries>();
        }

        /// <summary>
        /// Injects PortEval's financial data calculators.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void AddCalculators(this IServiceCollection services)
        {
            services.AddScoped<IInstrumentProfitCalculator, InstrumentProfitCalculator>();
            services.AddScoped<IInstrumentPerformanceCalculator, InstrumentPerformanceCalculator>();
            services.AddScoped<IPositionValueCalculator, PositionValueCalculator>();
            services.AddScoped<IPositionProfitCalculator, PositionProfitCalculator>();
            services.AddScoped<IPositionPerformanceCalculator, PositionPerformanceCalculator>();
            services.AddScoped<IPositionBreakEvenPointCalculator, PositionBreakEvenPointCalculator>();
            services.AddScoped<IPositionStatisticsCalculator, PositionStatisticsCalculator>();
            services.AddScoped<IPortfolioStatisticsCalculator, PortfolioStatisticsCalculator>();
        }

        /// <summary>
        /// Injects PortEval's chart data generators.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void AddChartGenerators(this IServiceCollection services)
        {
            services.AddScoped<IInstrumentChartDataGenerator, InstrumentChartDataGenerator>();
            services.AddScoped<IPositionChartDataGenerator, PositionChartDataGenerator>();
            services.AddScoped<IPortfolioChartDataGenerator, PortfolioChartDataGenerator>();
        }

        /// <summary>
        /// Injects PortEval's domain services.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyDomainService, CurrencyDomainService>();
        }

        /// <summary>
        /// Injects PortEval's domain event handlers.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void AddDomainEventHandlers(this IServiceCollection services)
        {
            services.AddMediatR(typeof(IDomainEventHandler<>));
        }

        /// <summary>
        /// Configures and injects EF Core database context.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        /// <param name="configuration">ASP.NET application configuration.</param>
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PortEvalDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("PortEvalDb")));
        }

        /// <summary>
        /// Configures and injects Dapper.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        public static void ConfigureDapper(this IServiceCollection services)
        {
            services.AddScoped<IDbConnectionCreator, PortEvalDbConnection>();
            Dapper.SqlMapper.AddTypeHandler(new ColorHandler());
            Dapper.SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
        }

        /// <summary>
        /// Configures and injects Hangfire.
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
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            services.AddHangfireServer();
        }

        /// <summary>
        /// Configures and injects internal instrument price and exchange rate fetching library.
        /// </summary>
        /// <param name="services">ASP.NET service IoC container.</param>
        /// <param name="configuration">ASP.NET application configuration.</param>
        public static void ConfigurePriceFetcher(this IServiceCollection services, IConfiguration configuration)
        {
            var dataFetcher = new DataFetcher.DataFetcher();
            var mboumKey = configuration.GetConfigurationValue("PORTEVAL_RapidAPI_Mboum_Key");
            var tiingoKey = configuration.GetConfigurationValue("PORTEVAL_Tiingo_Key");
            var openExchangeRatesKey = configuration.GetConfigurationValue("PORTEVAL_OpenExchangeRates_Key");

            if (tiingoKey != null)
            {
                dataFetcher.RegisterDataSource<TiingoApi>(new DataSourceConfiguration
                {
                    Credentials = new DataSourceCredentials
                    {
                        Token = tiingoKey
                    }
                });
            }

            if (mboumKey != null)
            {
                dataFetcher.RegisterDataSource<MboumApi>(new DataSourceConfiguration
                {
                    Credentials = new DataSourceCredentials
                    {
                        Token = mboumKey
                    }
                });
            }

            if (openExchangeRatesKey != null)
            {
                dataFetcher.RegisterDataSource<OpenExchangeRatesApi>(new DataSourceConfiguration
                {
                    Credentials = new DataSourceCredentials
                    {
                        Token = openExchangeRatesKey
                    }
                });
            }

            dataFetcher.RegisterDataSource<ExchangeRateHostApi>();
            services.AddSingleton<IDataFetcher>(dataFetcher);
            services.AddScoped<IFinancialDataFetcher, Infrastructure.FinancialDataFetcher.FinancialDataFetcher>();
        }

    }
}
