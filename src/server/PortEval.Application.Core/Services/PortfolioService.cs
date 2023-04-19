using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Services;

/// <inheritdoc cref="IPortfolioService" />
public class PortfolioService : IPortfolioService
{
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ICurrencyExchangeRateService _exchangeRateService;

    private readonly IPortfolioChartDataGenerator _chartDataGenerator;
    private readonly IPositionPerformanceCalculator _performanceCalculator;

    private readonly IPortfolioQueries _portfolioDataQueries;
    private readonly IPortfolioRepository _portfolioRepository;

    private readonly IPositionService _positionService;
    private readonly IPositionProfitCalculator _profitCalculator;
    private readonly IPortfolioStatisticsCalculator _statisticsCalculator;

    private readonly IPositionValueCalculator _valueCalculator;

    /// <summary>
    ///     Initializes the service.
    /// </summary>
    public PortfolioService(IPortfolioRepository portfolioRepository, ICurrencyRepository currencyRepository,
        IPositionService positionService, ICurrencyExchangeRateService exchangeRateService,
        IPortfolioQueries portfolioDataQueries, IPositionValueCalculator valueCalculator,
        IPositionProfitCalculator profitCalculator, IPositionPerformanceCalculator performanceCalculator,
        IPortfolioStatisticsCalculator statisticsCalculator, IPortfolioChartDataGenerator chartDataGenerator,
        ICurrencyConverter currencyConverter)
    {
        _portfolioRepository = portfolioRepository;
        _currencyRepository = currencyRepository;
        _positionService = positionService;
        _exchangeRateService = exchangeRateService;
        _portfolioDataQueries = portfolioDataQueries;
        _valueCalculator = valueCalculator;
        _profitCalculator = profitCalculator;
        _performanceCalculator = performanceCalculator;
        _statisticsCalculator = statisticsCalculator;
        _chartDataGenerator = chartDataGenerator;
        _currencyConverter = currencyConverter;
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<PortfolioDto>>> GetAllPortfoliosAsync()
    {
        var portfolios = await _portfolioDataQueries.GetAllPortfoliosAsync();
        return new OperationResponse<IEnumerable<PortfolioDto>>
        {
            Response = portfolios
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<PortfolioDto>> GetPortfolioAsync(int portfolioId)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);

        return new OperationResponse<PortfolioDto>
        {
            Status = portfolio != null ? OperationStatus.Ok : OperationStatus.NotFound,
            Message = portfolio != null ? "" : $"Portfolio {portfolioId} does not exist.",
            Response = portfolio
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<EntityValueDto>> GetPortfolioValueAsync(int portfolioId, DateTime time)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
            return new OperationResponse<EntityValueDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };

        var dateRange = new DateRangeParams
        {
            To = time
        };

        var portfolioPositionsPriceRangeData = await GetPositionsPriceRangeData(portfolio, dateRange);
        var value = _valueCalculator.CalculateValue(portfolioPositionsPriceRangeData.PositionsPriceRangeData, time);

        var valueDto = new EntityValueDto
        {
            Time = time,
            Value = value,
            CurrencyCode = portfolio.CurrencyCode
        };

        return new OperationResponse<EntityValueDto>
        {
            Status = OperationStatus.Ok,
            Response = valueDto
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<EntityProfitDto>> GetPortfolioProfitAsync(int portfolioId,
        DateRangeParams dateRange)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
            return new OperationResponse<EntityProfitDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };

        var portfolioPositionsPriceRangeData = await GetPositionsPriceRangeData(portfolio, dateRange);
        var profit = _profitCalculator.CalculateProfit(portfolioPositionsPriceRangeData.PositionsPriceRangeData,
            dateRange.From, dateRange.To);

        var profitDto = new EntityProfitDto
        {
            CurrencyCode = portfolio.CurrencyCode,
            From = dateRange.From,
            To = dateRange.To,
            Profit = profit
        };

        return new OperationResponse<EntityProfitDto>
        {
            Status = OperationStatus.Ok,
            Response = profitDto
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<EntityPerformanceDto>> GetPortfolioPerformanceAsync(int portfolioId,
        DateRangeParams dateRange)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
            return new OperationResponse<EntityPerformanceDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };

        var portfolioPositionsPriceRangeData = await GetPositionsPriceRangeData(portfolio, dateRange);
        var performance =
            _performanceCalculator.CalculatePerformance(portfolioPositionsPriceRangeData.PositionsPriceRangeData,
                dateRange.From, dateRange.To);

        return new OperationResponse<EntityPerformanceDto>
        {
            Status = OperationStatus.Ok,
            Response = new EntityPerformanceDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Performance = performance
            }
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioValueAsync(int portfolioId,
        DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };

        var positionsPriceData = await GetPositionsPriceListData(portfolio, dateRange.SetFrom(DateTime.MinValue));
        var result = _chartDataGenerator.ChartValue(positionsPriceData, dateRange, frequency);
        if (!string.Equals(portfolio.CurrencyCode, targetCurrencyCode, StringComparison.CurrentCultureIgnoreCase))
        {
            var exchangeRatesResponse =
                await _exchangeRateService.GetExchangeRatesAsync(portfolio.CurrencyCode, targetCurrencyCode,
                    dateRange.SetFrom(DateTime.MinValue));
            result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
        }

        return new OperationResponse<IEnumerable<EntityChartPointDto>>
        {
            Status = OperationStatus.Ok,
            Response = result
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioProfitAsync(int portfolioId,
        DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };

        var positionsPriceData = await GetPositionsPriceListData(portfolio, dateRange.SetFrom(DateTime.MinValue));
        var result = _chartDataGenerator.ChartProfit(positionsPriceData, dateRange, frequency);
        if (!string.Equals(portfolio.CurrencyCode, targetCurrencyCode, StringComparison.CurrentCultureIgnoreCase))
        {
            var exchangeRatesResponse =
                await _exchangeRateService.GetExchangeRatesAsync(portfolio.CurrencyCode, targetCurrencyCode,
                    dateRange.SetFrom(DateTime.MinValue));
            result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
        }

        return new OperationResponse<IEnumerable<EntityChartPointDto>>
        {
            Status = OperationStatus.Ok,
            Response = result
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioPerformanceAsync(
        int portfolioId, DateRangeParams dateRange, AggregationFrequency frequency)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };

        var positionsPriceData = await GetPositionsPriceListData(portfolio, dateRange.SetFrom(DateTime.MinValue));
        var result = _chartDataGenerator.ChartPerformance(positionsPriceData, dateRange, frequency);

        return new OperationResponse<IEnumerable<EntityChartPointDto>>
        {
            Status = OperationStatus.Ok,
            Response = result
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioAggregatedProfitAsync(
        int portfolioId,
        DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };

        var positionsPriceData = await GetPositionsPriceListData(portfolio, dateRange.SetFrom(DateTime.MinValue));
        var result = _chartDataGenerator.ChartAggregatedProfit(positionsPriceData, dateRange, frequency);
        if (!string.Equals(portfolio.CurrencyCode, targetCurrencyCode, StringComparison.CurrentCultureIgnoreCase))
        {
            var exchangeRatesResponse =
                await _exchangeRateService.GetExchangeRatesAsync(portfolio.CurrencyCode, targetCurrencyCode,
                    dateRange.SetFrom(DateTime.MinValue));
            result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
        }

        return new OperationResponse<IEnumerable<EntityChartPointDto>>
        {
            Status = OperationStatus.Ok,
            Response = result
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioAggregatedPerformanceAsync(
        int portfolioId, DateRangeParams dateRange, AggregationFrequency frequency)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };

        var positionsPriceData = await GetPositionsPriceListData(portfolio, dateRange.SetFrom(DateTime.MinValue));
        var result = _chartDataGenerator.ChartAggregatedPerformance(positionsPriceData, dateRange, frequency);

        return new OperationResponse<IEnumerable<EntityChartPointDto>>
        {
            Status = OperationStatus.Ok,
            Response = result
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<EntityStatisticsDto>>> GetAllPortfoliosStatisticsAsync()
    {
        var portfolios = await _portfolioDataQueries.GetAllPortfoliosAsync();

        var data = await Task.WhenAll(portfolios.Select(GetPortfolioStatistics));
        return new OperationResponse<IEnumerable<EntityStatisticsDto>>
        {
            Status = OperationStatus.Ok,
            Response = data.Select(stats => stats.Response)
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<EntityStatisticsDto>> GetPortfolioStatisticsAsync(int portfolioId)
    {
        var portfolio = await _portfolioDataQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
            return new OperationResponse<EntityStatisticsDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };

        return await GetPortfolioStatistics(portfolio);
    }

    /// <inheritdoc cref="IPortfolioService.CreatePortfolioAsync" />
    public async Task<OperationResponse<PortfolioDto>> CreatePortfolioAsync(PortfolioDto options)
    {
        if (!await _currencyRepository.ExistsAsync(options.CurrencyCode))
            return new OperationResponse<PortfolioDto>
            {
                Status = OperationStatus.Error,
                Message = $"Currency {options.CurrencyCode} does not exist."
            };

        var portfolio = _portfolioRepository.Add(Portfolio.Create(options.Name, options.Note, options.CurrencyCode));
        await _portfolioRepository.UnitOfWork.CommitAsync();
        return await GetPortfolioAsync(portfolio.Id);
    }

    /// <inheritdoc cref="IPortfolioService.UpdatePortfolioAsync" />
    public async Task<OperationResponse<PortfolioDto>> UpdatePortfolioAsync(PortfolioDto options)
    {
        var portfolio = await _portfolioRepository.FindAsync(options.Id);
        if (portfolio == null)
            return new OperationResponse<PortfolioDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {options.Id} does not exist."
            };

        if (!await _currencyRepository.ExistsAsync(options.CurrencyCode))
            return new OperationResponse<PortfolioDto>
            {
                Status = OperationStatus.Error,
                Message = $"Currency {options.CurrencyCode} does not exist."
            };

        portfolio.Rename(options.Name);
        portfolio.SetNote(options.Note);
        portfolio.ChangeCurrency(options.CurrencyCode);
        portfolio.IncreaseVersion();
        _portfolioRepository.Update(portfolio);

        await _portfolioRepository.UnitOfWork.CommitAsync();
        return await GetPortfolioAsync(portfolio.Id);
    }

    /// <inheritdoc cref="IPortfolioService.DeletePortfolioAsync" />
    public async Task<OperationResponse> DeletePortfolioAsync(int id)
    {
        if (!await _portfolioRepository.ExistsAsync(id))
            return new OperationResponse
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {id} does not exist."
            };

        await _portfolioRepository.DeleteAsync(id);
        await _portfolioRepository.UnitOfWork.CommitAsync();
        return new OperationResponse();
    }

    private async Task<OperationResponse<EntityStatisticsDto>> GetPortfolioStatistics(PortfolioDto portfolio)
    {
        var now = DateTime.UtcNow;

        var priceData = await GetPositionsPriceListData(portfolio, new DateRangeParams { To = now });
        var stats = _statisticsCalculator.CalculateStatistics(priceData, now);

        return new OperationResponse<EntityStatisticsDto>
        {
            Status = OperationStatus.Ok,
            Response = stats
        };
    }

    private async Task<PortfolioPositionsPriceRangeData> GetPositionsPriceRangeData(PortfolioDto portfolio,
        DateRangeParams dateRange)
    {
        var positions = await _positionService.GetPortfolioPositionsAsync(portfolio);

        var positionsPriceData = new List<PositionPriceRangeData>();

        foreach (var position in positions.Response)
        {
            var priceDataResponse = await _positionService.GetPositionPriceRangeDataAsync(position, dateRange);
            var priceData = priceDataResponse.Response;
            if (portfolio.CurrencyCode != position.Instrument.CurrencyCode)
            {
                var exchangeRates = await _exchangeRateService.GetExchangeRatesAsync(position.Instrument.CurrencyCode,
                    portfolio.CurrencyCode, dateRange.SetFrom(DateTime.MinValue));

                priceData.PositionId = position.Id;
                priceData.Transactions =
                    _currencyConverter.ConvertTransactions(priceData.Transactions, exchangeRates.Response);
                priceData.PriceAtRangeStart =
                    _currencyConverter
                        .ConvertInstrumentPrices(new[] { priceData.PriceAtRangeStart }, exchangeRates.Response).First();
                priceData.PriceAtRangeEnd =
                    _currencyConverter.ConvertInstrumentPrices(new[] { priceData.PriceAtRangeEnd },
                        exchangeRates.Response).First();
                priceData.DateRange = dateRange;
            }

            positionsPriceData.Add(priceData);
        }

        return new PortfolioPositionsPriceRangeData
        {
            PortfolioId = portfolio.Id,
            PositionsPriceRangeData = positionsPriceData
        };
    }

    private async Task<PortfolioPositionsPriceListData> GetPositionsPriceListData(PortfolioDto portfolio,
        DateRangeParams dateRange)
    {
        var positions = await _positionService.GetPortfolioPositionsAsync(portfolio);

        var positionsPriceListData = new List<PositionPriceListData>();

        foreach (var position in positions.Response)
        {
            var priceDataResponse = await _positionService.GetPositionPriceListDataAsync(position, dateRange);
            var priceData = priceDataResponse.Response;
            if (portfolio.CurrencyCode != position.Instrument.CurrencyCode)
            {
                var exchangeRates = await _exchangeRateService.GetExchangeRatesAsync(position.Instrument.CurrencyCode,
                    portfolio.CurrencyCode, dateRange.SetFrom(DateTime.MinValue));

                priceData.PositionId = position.Id;
                priceData.Transactions =
                    _currencyConverter.ConvertTransactions(priceData.Transactions, exchangeRates.Response);
                priceData.Prices =
                    _currencyConverter.ConvertInstrumentPrices(priceData.Prices, exchangeRates.Response);
            }

            positionsPriceListData.Add(priceData);
        }

        return new PortfolioPositionsPriceListData
        {
            PortfolioId = portfolio.Id,
            PositionsPriceListData = positionsPriceListData
        };
    }
}