using System;
using System.Data;
using System.IO.Abstractions;
using Dapper;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Core.BackgroundJobs;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Common.BulkImport;
using PortEval.Application.Core.Common.Calculators;
using PortEval.Application.Core.Common.ChartDataGenerators;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Interfaces;
using PortEval.Domain.Services;
using PortEval.Infrastructure;
using PortEval.Infrastructure.FinancialDataFetcher;
using PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage;
using PortEval.Infrastructure.FinancialDataFetcher.ExchangeRateHost;
using PortEval.Infrastructure.FinancialDataFetcher.OpenExchangeRates;
using PortEval.Infrastructure.FinancialDataFetcher.RapidAPIMboum;
using PortEval.Infrastructure.FinancialDataFetcher.Tiingo;
using PortEval.Infrastructure.Queries;
using PortEval.Infrastructure.Queries.TypeHandlers;
using PortEval.Infrastructure.Repositories;

namespace PortEval.Application.Extensions;

/// <summary>
///     Contains extension methods for PortEval's service configurations.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Injects PortEval's application services.
    /// </summary>
    /// <param name="services">ASP.NET service IoC container.</param>
    /// <param name="configuration">ASP.NET configuration.</param>
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IInstrumentService, InstrumentService>();
        services.AddScoped<IPortfolioService, PortfolioService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<ICurrencyExchangeRateService, CurrencyExchangeRateService>();
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
                new CsvImportService(
                    provider.GetRequiredService<IDataImportRepository>(),
                    provider.GetRequiredService<IBackgroundJobClient>(),
                    provider.GetRequiredService<IFileSystem>(),
                    provider.GetRequiredService<IDataImportQueries>(),
                    fileStoragePath)
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
    ///     Injects PortEval's repositories.
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
    ///     Injects PortEval's background jobs.
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
    ///     Injects PortEval's read queries.
    /// </summary>
    /// <param name="services">ASP.NET service IoC container.</param>
    public static void AddQueries(this IServiceCollection services)
    {
        services.AddScoped<IPortfolioQueries, PortfolioQueries>();
        services.AddScoped<IPositionQueries, PositionQueries>();
        services.AddScoped<ITransactionQueries, TransactionQueries>();
        services.AddScoped<IInstrumentQueries, InstrumentQueries>();
        services.AddScoped<ICurrencyQueries, CurrencyQueries>();
        services.AddScoped<IChartQueries, ChartQueries>();
        services.AddScoped<IDashboardLayoutQueries, DashboardLayoutQueries>();
        services.AddScoped<IDataImportQueries, DataImportQueries>();
        services.AddScoped<IExchangeQueries, ExchangeQueries>();
    }

    /// <summary>
    ///     Injects PortEval's financial data calculators.
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
    ///     Injects PortEval's chart data generators.
    /// </summary>
    /// <param name="services">ASP.NET service IoC container.</param>
    public static void AddChartGenerators(this IServiceCollection services)
    {
        services.AddScoped<IInstrumentChartDataGenerator, InstrumentChartDataGenerator>();
        services.AddScoped<IPositionChartDataGenerator, PositionChartDataGenerator>();
        services.AddScoped<IPortfolioChartDataGenerator, PortfolioChartDataGenerator>();
    }

    /// <summary>
    ///     Injects PortEval's domain services.
    /// </summary>
    /// <param name="services">ASP.NET service IoC container.</param>
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyDomainService, CurrencyDomainService>();
    }

    /// <summary>
    ///     Injects PortEval's domain event handlers.
    /// </summary>
    /// <param name="services">ASP.NET service IoC container.</param>
    public static void AddDomainEventHandlers(this IServiceCollection services)
    {
        services.AddMediatR(typeof(IDomainEventHandler<>));
    }

    /// <summary>
    ///     Configures and injects EF Core database context.
    /// </summary>
    /// <param name="services">ASP.NET service IoC container.</param>
    /// <param name="configuration">ASP.NET application configuration.</param>
    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PortEvalDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PortEvalDb")));
    }

    /// <summary>
    ///     Configures and injects Dapper.
    /// </summary>
    /// <param name="services">ASP.NET service IoC container.</param>
    public static void ConfigureDapper(this IServiceCollection services)
    {
        services.AddScoped<PortEvalDbConnectionCreator>();
        SqlMapper.AddTypeHandler(new ColorHandler());
        SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
    }

    /// <summary>
    ///     Configures and injects Hangfire.
    /// </summary>
    /// <param name="services">ASP.NET service IoC container.</param>
    /// <param name="configuration">ASP.NET application configuration.</param>
    public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(h => h.UseSqlServerStorage(configuration.GetConnectionString("PortEvalDb"),
            new SqlServerStorageOptions
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
    ///     Configures and injects internal instrument price and exchange rate fetching library.
    /// </summary>
    /// <param name="services">ASP.NET service IoC container.</param>
    /// <param name="configuration">ASP.NET application configuration.</param>
    public static void ConfigurePriceFetcher(this IServiceCollection services, IConfiguration configuration)
    {
        var dataFetcher = new DataFetcher.DataFetcher();
        var alphaVantageKey = configuration.GetConfigurationValue("PORTEVAL_AlphaVantage_Key");
        var mboumKey = configuration.GetConfigurationValue("PORTEVAL_RapidAPI_Mboum_Key");
        var tiingoKey = configuration.GetConfigurationValue("PORTEVAL_Tiingo_Key");
        var openExchangeRatesKey = configuration.GetConfigurationValue("PORTEVAL_OpenExchangeRates_Key");

        if (!string.IsNullOrWhiteSpace(alphaVantageKey))
        {
            dataFetcher.RegisterDataSource<AlphaVantageApi>(new DataSourceConfiguration
            {
                Credentials = new DataSourceCredentials
                {
                    Token = alphaVantageKey
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(tiingoKey))
        {
            dataFetcher.RegisterDataSource<TiingoApi>(new DataSourceConfiguration
            {
                Credentials = new DataSourceCredentials
                {
                    Token = tiingoKey
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(mboumKey))
        {
            dataFetcher.RegisterDataSource<MboumApi>(new DataSourceConfiguration
            {
                Credentials = new DataSourceCredentials
                {
                    Token = mboumKey
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(openExchangeRatesKey))
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
        services.AddScoped<IFinancialDataFetcher, FinancialDataFetcher>();
    }
}