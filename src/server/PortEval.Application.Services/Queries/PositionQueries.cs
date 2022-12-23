using Dapper;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Queries.DataQueries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Features.Extensions;

namespace PortEval.Application.Features.Queries
{
    /// <inheritdoc cref="IPositionQueries"/>
    public class PositionQueries : IPositionQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;
        private readonly ITransactionQueries _transactionQueries;
        private readonly IInstrumentQueries _instrumentQueries;

        private readonly IPositionValueCalculator _valueCalculator;
        private readonly IPositionProfitCalculator _profitCalculator;
        private readonly IPositionPerformanceCalculator _performanceCalculator;
        private readonly IPositionChartDataGenerator _chartGenerator;
        private readonly IPositionBreakEvenPointCalculator _breakEvenPointCalculator;

        private readonly ICurrencyConverter _currencyConverter;

        public PositionQueries(IDbConnectionCreator connection,
            ICurrencyExchangeRateQueries exchangeRateQueries, ITransactionQueries transactionQueries,
            IInstrumentQueries instrumentQueries, IPositionValueCalculator valueCalculator,
            IPositionProfitCalculator profitCalculator, IPositionPerformanceCalculator performanceCalculator,
            IPositionBreakEvenPointCalculator breakEvenPointCalculator, IPositionChartDataGenerator chartGenerator, ICurrencyConverter currencyConverter)
        {
            _connectionCreator = connection;
            _exchangeRateQueries = exchangeRateQueries;
            _transactionQueries = transactionQueries;
            _instrumentQueries = instrumentQueries;
            _valueCalculator = valueCalculator;
            _profitCalculator = profitCalculator;
            _performanceCalculator = performanceCalculator;
            _breakEvenPointCalculator = breakEvenPointCalculator;
            _chartGenerator = chartGenerator;
            _currencyConverter = currencyConverter;
        }

        /// <inheritdoc cref="IPositionQueries.GetAllPositions"/>
        public async Task<QueryResponse<IEnumerable<PositionDto>>> GetAllPositions()
        {
            var query = PositionDataQueries.GetPositions();

            using var connection = _connectionCreator.CreateConnection();
            var positions = await connection.QueryAsync<PositionDto, InstrumentDto, PositionDto>(query.Query, (p, i) =>
            {
                p.Instrument = i;
                return p;
            }, query.Params);

            return new QueryResponse<IEnumerable<PositionDto>>
            {
                Status = QueryStatus.Ok,
                Response = positions
            };
        }

        /// <inheritdoc cref="IPositionQueries.GetPortfolioPositions"/>
        public async Task<QueryResponse<IEnumerable<PositionDto>>> GetPortfolioPositions(int portfolioId)
        {
            // The portfolio query has to be done manually (and not through IPortfolioQueries) to prevent DI circular dependency.
            var portfolioQuery = PortfolioDataQueries.GetPortfolio(portfolioId);
            var positionsQuery = PositionDataQueries.GetPortfolioPositions(portfolioId);

            using var connection = _connectionCreator.CreateConnection();
            var portfolio =
                await connection.QueryFirstOrDefaultAsync<PortfolioDto>(portfolioQuery.Query, portfolioQuery.Params);
            if (portfolio == null)
            {
                return new QueryResponse<IEnumerable<PositionDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var positions = await connection.QueryAsync<PositionDto, InstrumentDto, PositionDto>(positionsQuery.Query, (p, i) =>
            {
                p.Instrument = i;
                return p;
            }, positionsQuery.Params);

            return new QueryResponse<IEnumerable<PositionDto>>
            {
                Status = QueryStatus.Ok,
                Response = positions
            };
        }

        /// <inheritdoc cref="IPositionQueries.GetPosition"/>
        public async Task<QueryResponse<PositionDto>> GetPosition(int positionId)
        {
            var query = PositionDataQueries.GetPosition(positionId);

            using var connection = _connectionCreator.CreateConnection();
            var positionQueryResponse = await connection.QueryAsync<PositionDto, InstrumentDto, PositionDto>(query.Query, (p, i) =>
            {
                p.Instrument = i;
                return p;
            }, query.Params);

            var position = positionQueryResponse.FirstOrDefault();
            return new QueryResponse<PositionDto>
            {
                Status = position != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = position
            };
        }

        /// <inheritdoc cref="IPositionQueries.GetPositionValue"/>
        public async Task<QueryResponse<EntityValueDto>> GetPositionValue(int positionId, DateTime time)
        {
            var position = await GetPosition(positionId);
            if (position.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityValueDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var dateRange = new DateRangeParams
            {
                To = time
            };

            var positionPriceData = await GetPositionPriceData(position.Response, dateRange);
            var value = _valueCalculator.CalculateValue(new [] { positionPriceData }, time);

            var result = new EntityValueDto
            {
                Time = time,
                Value = value,
                CurrencyCode = position.Response.Instrument.CurrencyCode
            };

            return new QueryResponse<EntityValueDto>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IPositionQueries.GetPositionProfit"/>
        public async Task<QueryResponse<EntityProfitDto>> GetPositionProfit(int positionId, DateRangeParams dateRange)
        {
            var position = await GetPosition(positionId);
            if (position.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityProfitDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var positionPriceRangeData = await GetPositionPriceRangeData(position.Response, dateRange);
            var profit = _profitCalculator.CalculateProfit(new[] { positionPriceRangeData }, dateRange.From,
                dateRange.To);

            var result = new EntityProfitDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Profit = profit,
                CurrencyCode = position.Response.Instrument.CurrencyCode
            };

            return new QueryResponse<EntityProfitDto>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IPositionQueries.GetPositionPerformance"/>
        public async Task<QueryResponse<EntityPerformanceDto>> GetPositionPerformance(int positionId, DateRangeParams dateRange)
        {
            var position = await GetPosition(positionId);
            if (position.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityPerformanceDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var positionPriceRangeData = await GetPositionPriceRangeData(position.Response, dateRange);
            var performance = _performanceCalculator.CalculatePerformance(new [] { positionPriceRangeData }, dateRange.From, dateRange.To);

            return new QueryResponse<EntityPerformanceDto>
            {
                Status = QueryStatus.Ok,
                Response = new EntityPerformanceDto
                {
                    From = dateRange.From,
                    To = dateRange.To,
                    Performance = performance
                }
            };
        }

        /// <inheritdoc cref="IPositionQueries.GetPositionBreakEvenPoint"/>
        public async Task<QueryResponse<PositionBreakEvenPointDto>> GetPositionBreakEvenPoint(int positionId, DateTime time)
        {
            var position = await GetPosition(positionId);
            if (position.Status != QueryStatus.Ok)
            {
                return new QueryResponse<PositionBreakEvenPointDto>
                {
                    Status = position.Status
                };
            }

            var transactions = await _transactionQueries.GetTransactions(
                TransactionFilters.FromPositionId(positionId),
                new DateRangeParams { To = time }
            );

            var bep = _breakEvenPointCalculator.CalculatePositionBreakEvenPoint(transactions.Response);

            return new QueryResponse<PositionBreakEvenPointDto>
            {
                Status = QueryStatus.Ok,
                Response = new PositionBreakEvenPointDto
                {
                    BreakEvenPoint = bep,
                    Time = time
                }
            };
        }

        /// <inheritdoc cref="IPositionQueries.ChartPositionValue"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionValue(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
        {
            var position = await GetPosition(positionId);
            if (position.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var positionPriceListData = await GetPositionPriceListData(position.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartValue(new[] { positionPriceListData }, dateRange, frequency);

            if (!string.IsNullOrWhiteSpace(targetCurrencyCode) && position.Response.Instrument.CurrencyCode != targetCurrencyCode)
            {
                var exchangeRatesResponse =
                    await _exchangeRateQueries.GetExchangeRates(position.Response.Instrument.CurrencyCode, targetCurrencyCode, dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IPositionQueries.ChartPositionProfit"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionProfit(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
        {
            var position = await GetPosition(positionId);
            if (position.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var positionPriceListData = await GetPositionPriceListData(position.Response, dateRange.SetFrom(DateTime.MinValue));

            var result = _chartGenerator.ChartProfit(new[] { positionPriceListData }, dateRange, frequency);

            if (!string.IsNullOrWhiteSpace(targetCurrencyCode) && position.Response.Instrument.CurrencyCode != targetCurrencyCode)
            {
                var exchangeRatesResponse =
                    await _exchangeRateQueries.GetExchangeRates(position.Response.Instrument.CurrencyCode, targetCurrencyCode, dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IPositionQueries.ChartPositionPerformance"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionPerformance(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var position = await GetPosition(positionId);
            if (position.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var positionPriceListData = await GetPositionPriceListData(position.Response, dateRange.SetFrom(DateTime.MinValue));

            var result = _chartGenerator.ChartPerformance(new[] { positionPriceListData }, dateRange, frequency);

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IPositionQueries.ChartPositionProfitAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionProfitAggregated(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
        {
            var position = await GetPosition(positionId);
            if (position.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var positionPriceListData = await GetPositionPriceListData(position.Response, dateRange.SetFrom(DateTime.MinValue));

            var result = _chartGenerator.ChartAggregatedProfit(new[] { positionPriceListData }, dateRange, frequency);

            if (!string.IsNullOrWhiteSpace(targetCurrencyCode) && position.Response.Instrument.CurrencyCode != targetCurrencyCode)
            {
                var exchangeRatesResponse =
                    await _exchangeRateQueries.GetExchangeRates(position.Response.Instrument.CurrencyCode, targetCurrencyCode, dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IPositionQueries.ChartPositionPerformanceAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPositionPerformanceAggregated(int positionId,
            DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var position = await GetPosition(positionId);
            if (position.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var positionPriceListData = await GetPositionPriceListData(position.Response, dateRange.SetFrom(DateTime.MinValue));

            var result = _chartGenerator.ChartAggregatedPerformance(new[] { positionPriceListData }, dateRange, frequency);

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IPositionQueries.GetPortfolioPositionsStatistics"/>
        public async Task<QueryResponse<IEnumerable<PositionStatisticsDto>>> GetPortfolioPositionsStatistics(int portfolioId)
        {
            var positions = await GetPortfolioPositions(portfolioId);

            var data = await Task.WhenAll(positions.Response.Select(GetPositionStatistics));
            return new QueryResponse<IEnumerable<PositionStatisticsDto>>
            {
                Status = QueryStatus.Ok,
                Response = data.Select(stats => stats.Response)
            };
        }

        /// <inheritdoc cref="IPositionQueries.GetPositionStatistics"/>
        public async Task<QueryResponse<PositionStatisticsDto>> GetPositionStatistics(int id)
        {
            var position = await GetPosition(id);
            if (position.Status != QueryStatus.Ok)
            {
                return new QueryResponse<PositionStatisticsDto>
                {
                    Status = position.Status
                };
            }

            return await GetPositionStatistics(position.Response);
        }

        /// <inheritdoc />
        public async Task<PositionPriceData> GetPositionPriceData(PositionDto position, DateRangeParams dateRange)
        {
            var price = await _instrumentQueries.GetInstrumentPrice(position.InstrumentId, dateRange.To);
            var transactions =
                await _transactionQueries.GetTransactions(TransactionFilters.FromPositionId(position.Id), dateRange.SetFrom(DateTime.MinValue));

            return new PositionPriceData
            {
                Price = price.Response,
                Time = dateRange.To,
                Transactions = transactions.Response
            };
        }

        /// <inheritdoc />
        public async Task<PositionPriceRangeData> GetPositionPriceRangeData(PositionDto position,
            DateRangeParams dateRange)
        {
            var transactions =
                await _transactionQueries.GetTransactions(TransactionFilters.FromPositionId(position.Id), dateRange.SetFrom(DateTime.MinValue));

            var firstTransactionTime = transactions.Response?.FirstOrDefault()?.Time;
            var adjustedDateRange = dateRange;
            if (firstTransactionTime != null)
            {
                adjustedDateRange = adjustedDateRange.SetFrom(adjustedDateRange.From.GetMax((DateTime)firstTransactionTime));
            }

            var priceAtStart = await _instrumentQueries.GetInstrumentPrice(position.InstrumentId, adjustedDateRange.From);
            var priceAtEnd = await _instrumentQueries.GetInstrumentPrice(position.InstrumentId, adjustedDateRange.To);
            
            return new PositionPriceRangeData
            {
                PriceAtRangeStart = priceAtStart.Response,
                PriceAtRangeEnd = priceAtEnd.Response,
                Transactions = transactions.Response
            };
        }

        /// <inheritdoc />
        public async Task<PositionPriceListData> GetPositionPriceListData(PositionDto position, DateRangeParams dateRange)
        {
            var prices = await _instrumentQueries.GetInstrumentPrices(position.InstrumentId, dateRange);
            var transactions =
                await _transactionQueries.GetTransactions(TransactionFilters.FromPositionId(position.Id), dateRange);

            return new PositionPriceListData
            {
                Prices = prices.Response,
                Transactions = transactions.Response
            };
        }

        private async Task<QueryResponse<PositionStatisticsDto>> GetPositionStatistics(PositionDto position)
        {
            var now = DateTime.UtcNow;

            var positionTotalPriceRangeData = await GetPositionPriceRangeData(position, new DateRangeParams { To = now });
            var positionLastDayPriceRangeData = await GetPositionPriceRangeData(position, new DateRangeParams { From = now.AddDays(-1), To = now });
            var positionLastWeekPriceRangeData = await GetPositionPriceRangeData(position, new DateRangeParams { From = now.AddDays(-7), To = now });
            var positionLastMonthPriceRangeData = await GetPositionPriceRangeData(position, new DateRangeParams { From = now.AddMonths(-1), To = now });

            var performanceTotal =
                _performanceCalculator.CalculatePerformance(new[] { positionTotalPriceRangeData }, DateTime.MinValue, now);
            var performanceLastDay = _performanceCalculator.CalculatePerformance(new[] { positionLastDayPriceRangeData }, now.AddDays(-1), now);
            var performanceLastWeek = _performanceCalculator.CalculatePerformance(new[] { positionLastWeekPriceRangeData }, now.AddDays(-7), now);
            var performanceLastMonth = _performanceCalculator.CalculatePerformance(new[] { positionLastMonthPriceRangeData }, now.AddMonths(-1), now);

            var profitTotal = _profitCalculator.CalculateProfit(new[] { positionTotalPriceRangeData }, DateTime.MinValue, now);
            var profitLastDay = _profitCalculator.CalculateProfit(new[] { positionLastDayPriceRangeData }, now.AddDays(-1), now);
            var profitLastWeek = _profitCalculator.CalculateProfit(new[] { positionLastWeekPriceRangeData }, now.AddDays(-7), now);
            var profitLastMonth = _profitCalculator.CalculateProfit(new[] { positionLastMonthPriceRangeData }, now.AddMonths(-1), now);

            var bep = _breakEvenPointCalculator.CalculatePositionBreakEvenPoint(positionTotalPriceRangeData.Transactions);

            return new QueryResponse<PositionStatisticsDto>
            {
                Status = QueryStatus.Ok,
                Response = new PositionStatisticsDto
                {
                    Id = position.Id,
                    TotalPerformance = performanceTotal,
                    LastDayPerformance = performanceLastDay,
                    LastWeekPerformance = performanceLastWeek,
                    LastMonthPerformance = performanceLastMonth,
                    TotalProfit = profitTotal,
                    LastDayProfit = profitLastDay,
                    LastWeekProfit = profitLastWeek,
                    LastMonthProfit = profitLastMonth,
                    BreakEvenPoint = bep
                }
            };
        } 
    }
}
