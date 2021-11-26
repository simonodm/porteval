using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Queries.DataQueries;
using PortEval.Application.Queries.Helpers;
using PortEval.Application.Queries.Interfaces;
using PortEval.Application.Queries.Models;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Queries
{
    /// <inheritdoc cref="IPositionQueries"/>
    public class PositionQueries : IPositionQueries
    {
        private readonly PortEvalDbConnection _connection;
        private readonly IInstrumentQueries _instrumentQueries;
        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;
        private readonly ITransactionQueries _transactionQueries;

        public PositionQueries(PortEvalDbConnection connection,
            IInstrumentQueries instrumentQueries, ICurrencyExchangeRateQueries exchangeRateQueries, ITransactionQueries transactionQueries)
        {
            _connection = connection;
            _instrumentQueries = instrumentQueries;
            _exchangeRateQueries = exchangeRateQueries;
            _transactionQueries = transactionQueries;
        }

        /// <inheritdoc cref="IPositionQueries.GetPortfolioPositions"/>
        public async Task<QueryResponse<IEnumerable<PositionDto>>> GetPortfolioPositions(int portfolioId)
        {
            // The portfolio query has to be done manually (and not through IPortfolioQueries) to prevent DI circular dependency.
            var portfolioQuery = PortfolioDataQueries.GetPortfolio(portfolioId);
            var positionsQuery = PositionDataQueries.GetPortfolioPositionsWithInstruments(portfolioId);

            using var connection = _connection.CreateConnection();
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

            using var connection = _connection.CreateConnection();
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

            var transactions = await _transactionQueries.GetPositionTransactions(positionId,
                new DateRangeParams { 
                    From = DateTime.MinValue,
                    To = time
                });

            var price = await _instrumentQueries.GetInstrumentPrice(position.Response.InstrumentId, time);

            decimal amountSum = 0;
            foreach (var transaction in transactions.Response)
            {
                amountSum += transaction.Amount;
            }

            var value = new EntityValueDto
            {
                Time = time,
                Value = amountSum * (price.Response?.Price ?? 0)
            };

            return new QueryResponse<EntityValueDto>
            {
                Status = QueryStatus.Ok,
                Response = value
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

            var transactions =
                await _transactionQueries.GetPositionTransactions(positionId, dateRange.SetFrom(DateTime.MinValue));

            var instrumentPriceAtRangeStart =
                (await _instrumentQueries.GetInstrumentPrice(position.Response.InstrumentId, dateRange.From)).Response?.Price ?? 0m;
            var instrumentPriceAtRangeEnd =
                (await _instrumentQueries.GetInstrumentPrice(position.Response.InstrumentId, dateRange.To)).Response?.Price ?? 0m;

            var profitAtRangeEnd = 0m;
            var profitAtRangeStart = 0m;
            foreach (var transaction in transactions.Response)
            {
                if (transaction.Time <= dateRange.From)
                {
                    profitAtRangeStart += (transaction.Amount * instrumentPriceAtRangeStart) -
                                          (transaction.Amount * transaction.Price);
                }
                profitAtRangeEnd += (transaction.Amount * instrumentPriceAtRangeEnd) -
                                    (transaction.Amount * transaction.Price);
            }

            var profit = new EntityProfitDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Profit = profitAtRangeEnd - profitAtRangeStart
            };

            return new QueryResponse<EntityProfitDto>
            {
                Status = QueryStatus.Ok,
                Response = profit
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

            var firstTransactionTime = await GetFirstTransactionTime(positionId);

            if (firstTransactionTime == null)
            {
                var zeroPerformance = new EntityPerformanceDto
                {
                    From = dateRange.From,
                    To = dateRange.To,
                    Performance = 0
                };
                return new QueryResponse<EntityPerformanceDto>
                {
                    Status = QueryStatus.Ok,
                    Response = zeroPerformance
                };
            }

            IEnumerable<TransactionDetailsQueryModel> transactionData;
            using (var connection = _connection.CreateConnection())
            {
                var timeFrom = new DateTime(Math.Max(dateRange.From.Ticks, ((DateTime)firstTransactionTime).Ticks));

                var transactionDetailsQuery =
                    PositionDataQueries.GetDetailedTransactionsQuery(positionId, timeFrom, dateRange.To);
                transactionData = await connection.QueryAsync<TransactionDetailsQueryModel>(transactionDetailsQuery.Query,
                    transactionDetailsQuery.Params);
            }

            var performance = IrrPerformanceCalculator.CalculatePerformance(transactionData, dateRange.From, dateRange.To);
            return new QueryResponse<EntityPerformanceDto>
            {
                Status = QueryStatus.Ok,
                Response = performance
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

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(position.Response.Instrument.CurrencyCode,
                    targetCurrencyCode, EntityChartPointDto.FromValue((await GetPositionValue(positionId, range.To)).Response))
            );

            calculations = calculations.Prepend(await _exchangeRateQueries.ConvertChartPointCurrency(
                position.Response.Instrument.CurrencyCode,
                targetCurrencyCode,
                EntityChartPointDto.FromValue((await GetPositionValue(positionId, dateRange.From)).Response)));

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
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

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(position.Response.Instrument.CurrencyCode,
                    targetCurrencyCode, EntityChartPointDto.FromProfit((await GetPositionProfit(positionId, range.SetFrom(dateRange.From))).Response))
            );

            calculations = calculations.Prepend(new EntityChartPointDto(dateRange.From, 0));

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
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

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => EntityChartPointDto.FromPerformance((await GetPositionPerformance(positionId, range.SetFrom(dateRange.From))).Response)
            );

            calculations = calculations.Prepend(new EntityChartPointDto(dateRange.From, 0));

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
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

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(position.Response.Instrument.CurrencyCode,
                    targetCurrencyCode, EntityChartPointDto.FromProfit((await GetPositionProfit(positionId, range)).Response))
            );

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
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

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => EntityChartPointDto.FromPerformance((await GetPositionPerformance(positionId, range)).Response)
            );

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <summary>
        /// Retrieves the time of the first available transaction of the given position.
        /// </summary>
        /// <param name="portfolioId">Parent portfolio ID.</param>
        /// <param name="positionId">Position ID.</param>
        /// <returns>A task representing the asynchronous database query operation. Task result contains the time of the first transaction if such exists, null otherwise.</returns>
        private async Task<DateTime?> GetFirstTransactionTime(int positionId)
        {
            var query = PositionDataQueries.GetPositionFirstTransactionTime(positionId);

            using var connection = _connection.CreateConnection();
            var time = await connection.QueryFirstOrDefaultAsync<SingleTimeQueryModel>(query.Query, query.Params);

            return time.Time;
        }
    }
}
