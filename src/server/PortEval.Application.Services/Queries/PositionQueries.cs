﻿using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Queries.DataQueries;
using PortEval.Application.Services.Queries.Helpers;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Application.Services.Queries.Models;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Domain;
using PortEval.Application.Services.Extensions;
using PortEval.Application.Services.Queries.Calculators;
using PortEval.Application.Services.Queries.Calculators.Interfaces;

namespace PortEval.Application.Services.Queries
{
    /// <inheritdoc cref="IPositionQueries"/>
    public class PositionQueries : IPositionQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;
        private readonly IInstrumentQueries _instrumentQueries;
        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;
        private readonly ITransactionQueries _transactionQueries;

        private readonly ITransactionBasedValueCalculator _valueCalculator;
        private readonly ITransactionBasedProfitCalculator _profitCalculator;
        private readonly ITransactionBasedPerformanceCalculator _performanceCalculator;

        public PositionQueries(IDbConnectionCreator connection,
            IInstrumentQueries instrumentQueries, ICurrencyExchangeRateQueries exchangeRateQueries, ITransactionQueries transactionQueries)
        {
            _connectionCreator = connection;
            _instrumentQueries = instrumentQueries;
            _exchangeRateQueries = exchangeRateQueries;
            _transactionQueries = transactionQueries;

            _valueCalculator = new TransactionBasedValueCalculator();
            _profitCalculator = new TransactionBasedProfitCalculator();
            _performanceCalculator = new IrrPerformanceCalculator();
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

            using var connection = _connectionCreator.CreateConnection();
            var transactions = await GetDetailedTransactionData(connection, positionId, new DateRangeParams { To = time });

            var value = _valueCalculator.CalculateValue(transactions, time);

            var result = new EntityValueDto
            {
                Time = time,
                Value = value
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

            using var connection = _connectionCreator.CreateConnection();
            var transactions = await GetDetailedTransactionData(connection, positionId, dateRange);

            var profit = _profitCalculator.CalculateProfit(transactions, dateRange.From, dateRange.To);

            var result = new EntityProfitDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Profit = profit
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

            using var connection = _connectionCreator.CreateConnection();
            var transactions = await GetDetailedTransactionData(connection, positionId, dateRange);

            var performance = _performanceCalculator.CalculatePerformance(transactions, dateRange.From, dateRange.To);

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
            if(position.Status != QueryStatus.Ok)
            {
                return new QueryResponse<PositionBreakEvenPointDto>
                {
                    Status = position.Status
                };
            }

            var transactions = await _transactionQueries.GetTransactions(new TransactionFilters { PositionId = positionId }, new DateRangeParams { To = time });
            var totalPositionBuyingPrice = transactions.Response.Sum(t => t.Amount * t.Price);
            var positionAmount = transactions.Response.Sum(t => t.Amount);
            var bep = positionAmount != 0 ? totalPositionBuyingPrice / positionAmount : 0;

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

            var firstTransactionTime = await GetFirstTransactionTime(positionId);
            if(firstTransactionTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }
            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstTransactionTime));

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

            var firstTransactionTime = await GetFirstTransactionTime(positionId);
            if (firstTransactionTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }
            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstTransactionTime));

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

            var firstTransactionTime = await GetFirstTransactionTime(positionId);
            if (firstTransactionTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }
            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstTransactionTime));

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

            var firstTransactionTime = await GetFirstTransactionTime(positionId);
            if (firstTransactionTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }
            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstTransactionTime));

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

            var firstTransactionTime = await GetFirstTransactionTime(positionId);
            if (firstTransactionTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }
            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstTransactionTime));

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

        /// <inheritdoc cref="IPositionQueries.GetPortfolioPositionsStatistics"/>
        public async Task<QueryResponse<IEnumerable<PositionStatisticsDto>>> GetPortfolioPositionsStatistics(int portfolioId)
        {
            var now = DateTime.UtcNow;

            var positions = await GetPortfolioPositions(portfolioId);

            var data = await Task.WhenAll(positions.Response.Select(position => GetPositionStatistics(position.Id)));
            return new QueryResponse<IEnumerable<PositionStatisticsDto>>
            {
                Status = QueryStatus.Ok,
                Response = data.Select(stats => stats.Response)
            };
        }

        /// <inheritdoc cref="IPositionQueries.GetPositionStatistics"/>
        public async Task<QueryResponse<PositionStatisticsDto>> GetPositionStatistics(int id)
        {
            var now = DateTime.UtcNow;
            var performanceTotal = await GetPositionPerformance(id, new DateRangeParams { To = now });
            var performanceLastDay = await GetPositionPerformance(id, new DateRangeParams { From = now.AddDays(-1), To = now });
            var performanceLastWeek = await GetPositionPerformance(id, new DateRangeParams { From = now.AddDays(-7), To = now });
            var performanceLastMonth = await GetPositionPerformance(id, new DateRangeParams { From = now.AddMonths(-1), To = now });

            var profitTotal = await GetPositionProfit(id, new DateRangeParams { To = now });
            var profitLastDay = await GetPositionProfit(id, new DateRangeParams { From = now.AddDays(-1), To = now });
            var profitLastWeek = await GetPositionProfit(id, new DateRangeParams { From = now.AddDays(-7), To = now });
            var profitLastMonth = await GetPositionProfit(id, new DateRangeParams { From = now.AddMonths(-1), To = now });

            var bep = await GetPositionBreakEvenPoint(id, now);

            if (profitLastMonth.Status != QueryStatus.Ok)
            {
                return new QueryResponse<PositionStatisticsDto>
                {
                    Status = profitLastMonth.Status
                };
            }

            return new QueryResponse<PositionStatisticsDto>
            {
                Status = QueryStatus.Ok,
                Response = new PositionStatisticsDto
                {
                    Id = id,
                    TotalPerformance = performanceTotal.Response?.Performance ?? 0m,
                    LastDayPerformance = performanceLastDay.Response?.Performance ?? 0m,
                    LastWeekPerformance = performanceLastWeek.Response?.Performance ?? 0m,
                    LastMonthPerformance = performanceLastMonth.Response?.Performance ?? 0m,
                    TotalProfit = profitTotal.Response?.Profit ?? 0m,
                    LastDayProfit = profitLastDay.Response?.Profit ?? 0m,
                    LastWeekProfit = profitLastWeek.Response?.Profit ?? 0m,
                    LastMonthProfit = profitLastMonth.Response?.Profit ?? 0m,
                    BreakEvenPoint = bep.Response?.BreakEvenPoint ?? 0m
                }
            };
        }

        /// <summary>
        /// Retrieves the time of the first available transaction of the given position.
        /// </summary>
        /// <param name="positionId">Position ID.</param>
        /// <returns>A task representing the asynchronous database query operation. Task result contains the time of the first transaction if such exists, null otherwise.</returns>
        private async Task<DateTime?> GetFirstTransactionTime(int positionId)
        {
            var query = PositionDataQueries.GetPositionFirstTransactionTime(positionId);

            using var connection = _connectionCreator.CreateConnection();
            var time = await connection.QueryFirstOrDefaultAsync<SingleTimeQueryModel>(query.Query, query.Params);

            return time.Time;
        }

        private async Task<IEnumerable<TransactionDetailsQueryModel>> GetDetailedTransactionData(IDbConnection connection, int positionId, DateRangeParams dateRange)
        {
            var transactionDetailsQuery =
                PositionDataQueries.GetDetailedTransactionsQuery(positionId, dateRange.From, dateRange.To);
            var transactionData = await connection.QueryAsync<TransactionDetailsQueryModel>(transactionDetailsQuery.Query,
                transactionDetailsQuery.Params);

            return transactionData;
        }
    }
}
