using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Services;

/// <inheritdoc cref="IPositionService" />
public class PositionService : IPositionService
{
    private readonly IPositionBreakEvenPointCalculator _breakEvenPointCalculator;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICurrencyExchangeRateService _exchangeRateService;

    private readonly IPositionChartDataGenerator _chartDataGenerator;
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IPositionPerformanceCalculator _performanceCalculator;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IPortfolioQueries _portfolioQueries;
    private readonly IPositionQueries _positionDataQueries;
    private readonly IPositionRepository _positionRepository;
    private readonly IInstrumentPriceService _priceService;
    private readonly IPositionProfitCalculator _profitCalculator;
    private readonly IPositionStatisticsCalculator _statisticsCalculator;

    private readonly ITransactionService _transactionService;

    private readonly IPositionValueCalculator _valueCalculator;

    /// <summary>
    ///     Initializes the service.
    /// </summary>
    public PositionService(IPortfolioRepository portfolioRepository, IPortfolioQueries portfolioQueries,
        IPositionRepository positionRepository, IInstrumentRepository instrumentRepository,
        IPositionQueries positionDataQueries, ITransactionService transactionService,
        IInstrumentPriceService priceService, ICurrencyExchangeRateService exchangeRateService,
        IPositionValueCalculator valueCalculator, IPositionProfitCalculator profitCalculator,
        IPositionPerformanceCalculator performanceCalculator,
        IPositionBreakEvenPointCalculator breakEvenPointCalculator, IPositionStatisticsCalculator statisticsCalculator,
        IPositionChartDataGenerator chartDataGenerator, ICurrencyConverter currencyConverter)
    {
        _instrumentRepository = instrumentRepository;
        _positionDataQueries = positionDataQueries;
        _transactionService = transactionService;
        _priceService = priceService;
        _exchangeRateService = exchangeRateService;
        _valueCalculator = valueCalculator;
        _profitCalculator = profitCalculator;
        _performanceCalculator = performanceCalculator;
        _breakEvenPointCalculator = breakEvenPointCalculator;
        _statisticsCalculator = statisticsCalculator;
        _chartDataGenerator = chartDataGenerator;
        _currencyConverter = currencyConverter;
        _positionRepository = positionRepository;
        _portfolioRepository = portfolioRepository;
        _portfolioQueries = portfolioQueries;
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<PositionDto>>> GetAllPositionsAsync()
    {
        var positions = await _positionDataQueries.GetAllPositionsAsync();

        return new OperationResponse<IEnumerable<PositionDto>>
        {
            Response = positions
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<PositionDto>>> GetPortfolioPositionsAsync(int portfolioId)
    {
        var portfolio = await _portfolioQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
        {
            return new OperationResponse<IEnumerable<PositionDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };
        }

        var positions = await _positionDataQueries.GetPortfolioPositionsAsync(portfolioId);
        return new OperationResponse<IEnumerable<PositionDto>>
        {
            Response = positions
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<PositionDto>>> GetPortfolioPositionsAsync(PortfolioDto portfolio)
    {
        var positions = await _positionDataQueries.GetPortfolioPositionsAsync(portfolio.Id);
        return new OperationResponse<IEnumerable<PositionDto>>
        {
            Response = positions
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<PositionDto>> GetPositionAsync(int positionId)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);

        return new OperationResponse<PositionDto>
        {
            Status = position != null ? OperationStatus.Ok : OperationStatus.NotFound,
            Message = position != null ? "" : $"Position {positionId} does not exist.",
            Response = position
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<EntityValueDto>> GetPositionValueAsync(int positionId, DateTime time)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<EntityValueDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        var dateRange = new DateRangeParams
        {
            To = time
        };

        var positionPriceData = await GetPositionPriceRangeDataAsync(position, dateRange);
        var value = _valueCalculator.CalculateValue(new[] { positionPriceData.Response }, time);

        return new OperationResponse<EntityValueDto>
        {
            Status = OperationStatus.Ok,
            Response = new EntityValueDto
            {
                Time = time,
                Value = value,
                CurrencyCode = position.Instrument.CurrencyCode
            }
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<EntityProfitDto>> GetPositionProfitAsync(int positionId,
        DateRangeParams dateRange)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<EntityProfitDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        var positionPriceRangeData =
            await GetPositionPriceRangeDataAsync(position, dateRange);
        var profit = _profitCalculator.CalculateProfit(new[] { positionPriceRangeData.Response }, dateRange.From,
            dateRange.To);

        return new OperationResponse<EntityProfitDto>
        {
            Status = OperationStatus.Ok,
            Response = new EntityProfitDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Profit = profit,
                CurrencyCode = position.Instrument.CurrencyCode
            }
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<EntityPerformanceDto>> GetPositionPerformanceAsync(int positionId,
        DateRangeParams dateRange)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<EntityPerformanceDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        var positionPriceRangeData =
            await GetPositionPriceRangeDataAsync(position, dateRange);
        var performance = _performanceCalculator.CalculatePerformance(new[] { positionPriceRangeData.Response },
            dateRange.From,
            dateRange.To);

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
    public async Task<OperationResponse<PositionBreakEvenPointDto>> GetPositionBreakEvenPointAsync(int positionId,
        DateTime time)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<PositionBreakEvenPointDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        var transactions =
            await _transactionService.GetTransactionsAsync(TransactionFilters.FromPositionId(positionId),
                new DateRangeParams { To = time });

        var bep = _breakEvenPointCalculator.CalculatePositionBreakEvenPoint(transactions.Response);

        return new OperationResponse<PositionBreakEvenPointDto>
        {
            Status = OperationStatus.Ok,
            Response = new PositionBreakEvenPointDto
            {
                BreakEvenPoint = bep,
                Time = time
            }
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionValueAsync(int positionId,
        DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        var positionPriceListData = await GetPositionPriceListDataAsync(position, dateRange.SetFrom(DateTime.MinValue));
        var result = _chartDataGenerator.ChartValue(positionPriceListData.Response, dateRange, frequency);

        if (!string.Equals(position.Instrument.CurrencyCode, targetCurrencyCode,
                StringComparison.CurrentCultureIgnoreCase))
        {
            var exchangeRatesResponse =
                await _exchangeRateService.GetExchangeRatesAsync(position.Instrument.CurrencyCode, targetCurrencyCode,
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
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionProfitAsync(int positionId,
        DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        var positionPriceListData = await GetPositionPriceListDataAsync(position, dateRange.SetFrom(DateTime.MinValue));

        var result = _chartDataGenerator.ChartProfit(positionPriceListData.Response, dateRange, frequency);

        if (!string.Equals(position.Instrument.CurrencyCode, targetCurrencyCode,
                StringComparison.CurrentCultureIgnoreCase))
        {
            var exchangeRatesResponse =
                await _exchangeRateService.GetExchangeRatesAsync(position.Instrument.CurrencyCode, targetCurrencyCode,
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
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionPerformanceAsync(int positionId,
        DateRangeParams dateRange, AggregationFrequency frequency)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        var positionPriceListData = await GetPositionPriceListDataAsync(position, dateRange.SetFrom(DateTime.MinValue));

        var result = _chartDataGenerator.ChartPerformance(positionPriceListData.Response, dateRange, frequency);

        return new OperationResponse<IEnumerable<EntityChartPointDto>>
        {
            Status = OperationStatus.Ok,
            Response = result
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionAggregatedProfitAsync(
        int positionId,
        DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        var positionPriceListData = await GetPositionPriceListDataAsync(position, dateRange.SetFrom(DateTime.MinValue));

        var result = _chartDataGenerator.ChartAggregatedProfit(positionPriceListData.Response, dateRange, frequency);

        if (!string.Equals(position.Instrument.CurrencyCode, targetCurrencyCode,
                StringComparison.CurrentCultureIgnoreCase))
        {
            var exchangeRatesResponse =
                await _exchangeRateService.GetExchangeRatesAsync(position.Instrument.CurrencyCode, targetCurrencyCode,
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
    public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartPositionAggregatedPerformanceAsync(
        int positionId,
        DateRangeParams dateRange, AggregationFrequency frequency)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        var positionPriceListData = await GetPositionPriceListDataAsync(position, dateRange.SetFrom(DateTime.MinValue));

        var result =
            _chartDataGenerator.ChartAggregatedPerformance(positionPriceListData.Response, dateRange, frequency);

        return new OperationResponse<IEnumerable<EntityChartPointDto>>
        {
            Status = OperationStatus.Ok,
            Response = result
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<PositionStatisticsDto>>> GetPortfolioPositionsStatisticsAsync(
        int portfolioId)
    {
        var portfolio = await _portfolioQueries.GetPortfolioAsync(portfolioId);
        if (portfolio == null)
        {
            return new OperationResponse<IEnumerable<PositionStatisticsDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Portfolio {portfolioId} does not exist."
            };
        }

        var positions = await _positionDataQueries.GetPortfolioPositionsAsync(portfolioId);

        var data = await Task.WhenAll(positions.Select(GetPositionStatistics));
        return new OperationResponse<IEnumerable<PositionStatisticsDto>>
        {
            Status = OperationStatus.Ok,
            Response = data.Select(stats => stats.Response)
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<PositionStatisticsDto>> GetPositionStatisticsAsync(int positionId)
    {
        var position = await _positionDataQueries.GetPositionAsync(positionId);
        if (position == null)
        {
            return new OperationResponse<PositionStatisticsDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        return await GetPositionStatistics(position);
    }

    /// <inheritdoc />
    public async Task<OperationResponse<PositionDto>> OpenPositionAsync(PositionDto options)
    {
        if (options.Amount == null || options.Price == null || options.Time == null)
        {
            return new OperationResponse<PositionDto>
            {
                Status = OperationStatus.Error,
                Message = "An initial transaction is required when opening a new position."
            };
        }

        var initialTransactionTime = (DateTime)options.Time;
        var initialTransactionPrice = (decimal)options.Price;
        var initialTransactionAmount = (decimal)options.Amount;

        var portfolio = await _portfolioRepository.FindAsync(options.PortfolioId);
        if (portfolio == null)
        {
            return new OperationResponse<PositionDto>
            {
                Status = OperationStatus.Error,
                Message = $"Portfolio {options.PortfolioId} does not exist."
            };
        }

        var instrument = await _instrumentRepository.FindAsync(options.InstrumentId);
        if (instrument == null)
        {
            return new OperationResponse<PositionDto>
            {
                Status = OperationStatus.Error,
                Message = $"Instrument {options.InstrumentId} does not exist."
            };
        }

        try
        {
            var createdPosition = Position.Create(portfolio, instrument, options.Note);
            createdPosition.AddTransaction(initialTransactionAmount, initialTransactionPrice,
                initialTransactionTime);
            _positionRepository.Add(createdPosition);
            await _positionRepository.UnitOfWork.CommitAsync();

            return await GetPositionAsync(createdPosition.Id);
        }
        catch (PortEvalException ex)
        {
            return new OperationResponse<PositionDto>
            {
                Status = OperationStatus.Error,
                Message = ex.Message
            };
        }
    }

    /// <inheritdoc />
    public async Task<OperationResponse<PositionDto>> UpdatePositionAsync(PositionDto options)
    {
        var position = await _positionRepository.FindAsync(options.Id);
        if (position == null)
        {
            return new OperationResponse<PositionDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {options.Id} does not exist."
            };
        }

        position.SetNote(options.Note);
        position.IncreaseVersion();
        _positionRepository.Update(position);
        await _positionRepository.UnitOfWork.CommitAsync();

        return await GetPositionAsync(position.Id);
    }

    /// <inheritdoc />
    public async Task<OperationResponse> RemovePositionAsync(int positionId)
    {
        if (!await _positionRepository.ExistsAsync(positionId))
        {
            return new OperationResponse
            {
                Status = OperationStatus.NotFound,
                Message = $"Position {positionId} does not exist."
            };
        }

        await _positionRepository.DeleteAsync(positionId);
        await _positionRepository.UnitOfWork.CommitAsync();
        return new OperationResponse();
    }

    /// <inheritdoc />
    public async Task<OperationResponse<PositionPriceRangeData>> GetPositionPriceRangeDataAsync(PositionDto position,
        DateRangeParams dateRange)
    {
        var transactions =
            await _transactionService.GetTransactionsAsync(TransactionFilters.FromPositionId(position.Id),
                dateRange.SetFrom(DateTime.MinValue));

        var firstTransactionTime = transactions?.Response?.FirstOrDefault()?.Time;
        var adjustedDateRange = dateRange;
        if (firstTransactionTime != null)
        {
            adjustedDateRange =
                adjustedDateRange.SetFrom(adjustedDateRange.From.GetMax((DateTime)firstTransactionTime));
        }

        var priceAtStart = await _priceService.GetInstrumentPriceAsync(position.InstrumentId, adjustedDateRange.From);
        var priceAtEnd = await _priceService.GetInstrumentPriceAsync(position.InstrumentId, adjustedDateRange.To);

        return new OperationResponse<PositionPriceRangeData>
        {
            Response = new PositionPriceRangeData
            {
                PositionId = position.Id,
                PriceAtRangeStart = priceAtStart.Response,
                PriceAtRangeEnd = priceAtEnd.Response,
                Transactions = transactions?.Response,
                DateRange = dateRange
            }
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<PositionPriceListData>> GetPositionPriceListDataAsync(PositionDto position,
        DateRangeParams dateRange)
    {
        var prices = await _priceService.GetInstrumentPricesAsync(position.Instrument, dateRange);
        var transactions =
            await _transactionService.GetTransactionsAsync(TransactionFilters.FromPositionId(position.Id), dateRange);

        return new OperationResponse<PositionPriceListData>
        {
            Response = new PositionPriceListData
            {
                PositionId = position.Id,
                Prices = prices.Response,
                Transactions = transactions?.Response
            }
        };
    }

    private async Task<OperationResponse<PositionStatisticsDto>> GetPositionStatistics(PositionDto position)
    {
        var now = DateTime.UtcNow;

        var priceData = await GetPositionPriceListDataAsync(position, new DateRangeParams { To = now });
        var stats = _statisticsCalculator.CalculateStatistics(priceData.Response, now);

        return new OperationResponse<PositionStatisticsDto>
        {
            Status = OperationStatus.Ok,
            Response = stats
        };
    }
}