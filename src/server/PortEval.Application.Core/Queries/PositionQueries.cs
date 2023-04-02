using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Queries.DataQueries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Queries
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

        private readonly IPositionStatisticsCalculator _statisticsCalculator;

        private readonly ICurrencyConverter _currencyConverter;

        public PositionQueries(IDbConnectionCreator connection,
            ICurrencyExchangeRateQueries exchangeRateQueries, ITransactionQueries transactionQueries,
            IInstrumentQueries instrumentQueries, IPositionValueCalculator valueCalculator,
            IPositionProfitCalculator profitCalculator, IPositionPerformanceCalculator performanceCalculator,
            IPositionBreakEvenPointCalculator breakEvenPointCalculator, IPositionChartDataGenerator chartGenerator, ICurrencyConverter currencyConverter,
            IPositionStatisticsCalculator statisticsCalculator)
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
            _statisticsCalculator = statisticsCalculator;
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

            var positionPriceData = await GetPositionPriceRangeData(position.Response, dateRange);
            var value = _valueCalculator.CalculateValue(new[] { positionPriceData }, time);

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
            var performance = _performanceCalculator.CalculatePerformance(new[] { positionPriceRangeData }, dateRange.From, dateRange.To);

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
            var result = _chartGenerator.ChartValue(positionPriceListData, dateRange, frequency);

            if (!string.Equals(position.Response.Instrument.CurrencyCode, targetCurrencyCode, StringComparison.CurrentCultureIgnoreCase))
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

            var result = _chartGenerator.ChartProfit(positionPriceListData, dateRange, frequency);

            if (!string.Equals(position.Response.Instrument.CurrencyCode, targetCurrencyCode, StringComparison.CurrentCultureIgnoreCase))
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

            var result = _chartGenerator.ChartPerformance(positionPriceListData, dateRange, frequency);

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

            var result = _chartGenerator.ChartAggregatedProfit(positionPriceListData, dateRange, frequency);

            if (!string.Equals(position.Response.Instrument.CurrencyCode, targetCurrencyCode, StringComparison.CurrentCultureIgnoreCase))
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

            var result = _chartGenerator.ChartAggregatedPerformance(positionPriceListData, dateRange, frequency);

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
                PositionId = position.Id,
                PriceAtRangeStart = priceAtStart.Response,
                PriceAtRangeEnd = priceAtEnd.Response,
                Transactions = transactions.Response,
                DateRange = dateRange
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
                PositionId = position.Id,
                Prices = prices.Response,
                Transactions = transactions.Response
            };
        }

        private async Task<QueryResponse<PositionStatisticsDto>> GetPositionStatistics(PositionDto position)
        {
            var now = DateTime.UtcNow;

            var priceData = await GetPositionPriceListData(position, new DateRangeParams { To = now });
            var stats = _statisticsCalculator.CalculateStatistics(priceData, now);

            return new QueryResponse<PositionStatisticsDto>
            {
                Status = QueryStatus.Ok,
                Response = stats
            };
        }
    }
}
