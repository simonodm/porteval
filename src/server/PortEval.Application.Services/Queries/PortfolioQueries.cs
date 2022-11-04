using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Extensions;
using PortEval.Application.Services.Queries.DataQueries;
using PortEval.Application.Services.Queries.Helpers;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Application.Services.Queries.Models;
using PortEval.Domain;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Queries
{
    /// <inheritdoc cref="IPortfolioQueries"/>
    public class PortfolioQueries : IPortfolioQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;
        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;
        private readonly IPositionQueries _positionQueries;

        public PortfolioQueries(IDbConnectionCreator connection, ICurrencyExchangeRateQueries exchangeRateQueries,
            IPositionQueries positionQueries)
        {
            _connectionCreator = connection;
            _exchangeRateQueries = exchangeRateQueries;
            _positionQueries = positionQueries;
        }

        /// <inheritdoc cref="IPortfolioQueries.GetPortfolios"/>
        public async Task<QueryResponse<IEnumerable<PortfolioDto>>> GetPortfolios()
        {
            var query = PortfolioDataQueries.GetAllPortfolios();

            using var connection = _connectionCreator.CreateConnection();
            var portfolios = await connection.QueryAsync<PortfolioDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<PortfolioDto>>
            {
                Status = QueryStatus.Ok,
                Response = portfolios
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.GetPortfolio"/>
        public async Task<QueryResponse<PortfolioDto>> GetPortfolio(int portfolioId)
        {
            var query = PortfolioDataQueries.GetPortfolio(portfolioId);

            using var connection = _connectionCreator.CreateConnection();
            var portfolio = await connection.QueryFirstOrDefaultAsync<PortfolioDto>(query.Query, query.Params);

            return new QueryResponse<PortfolioDto>
            {
                Status = portfolio != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = portfolio
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.GetPortfolioValue"/>
        public async Task<QueryResponse<EntityValueDto>> GetPortfolioValue(int portfolioId, DateTime time)
        {
            var portfolio = await GetPortfolio(portfolioId);
            if(portfolio.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityValueDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var positions = await _positionQueries.GetPortfolioPositions(portfolioId);

            decimal value = 0;
            foreach (var position in positions.Response)
            {
                var positionValue = await _positionQueries.GetPositionValue(position.Id, time);
                if (position.Instrument.CurrencyCode == portfolio.Response.CurrencyCode)
                {
                    value += positionValue.Response.Value;
                    continue;
                }

                var convertedValue = await _exchangeRateQueries.Convert(position.Instrument.CurrencyCode,
                    portfolio.Response.CurrencyCode, positionValue.Response.Value, time);
                value += convertedValue;
            }

            var valueDto = new EntityValueDto
            {
                Time = time,
                Value = value
            };

            return new QueryResponse<EntityValueDto>
            {
                Status = QueryStatus.Ok,
                Response = valueDto
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.GetPortfolioProfit"/>
        public async Task<QueryResponse<EntityProfitDto>> GetPortfolioProfit(int portfolioId, DateRangeParams dateRange)
        {
            var portfolio = await GetPortfolio(portfolioId);
            if (portfolio.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityProfitDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            List<PortfolioTransactionDetailsQueryModel> transactions;
            using (var connection = _connectionCreator.CreateConnection())
            {
                transactions = (await GetTransactionsConvertedToPortfolioCurrency(connection, portfolioId,
                    new DateRangeParams { To = dateRange.To }, dateRange)).ToList();
            }

            var valueAtRangeStart = 0m;
            var valueAtRangeEnd = 0m;
            var profitInRange = 0m;
            foreach (var transaction in transactions)
            {
                if (transaction.Time < dateRange.From)
                {
                    valueAtRangeStart += transaction.Amount * transaction.InstrumentPriceAtRangeStart;
                }
                else
                {
                    profitInRange -= transaction.Amount * transaction.Price; // realized gains
                }
                valueAtRangeEnd += transaction.Amount * transaction.InstrumentPriceAtRangeEnd;
            }

            profitInRange += valueAtRangeEnd; // unrealized gains
            var totalProfit = profitInRange - valueAtRangeStart;

            var profitDto = new EntityProfitDto
            {
                CurrencyCode = portfolio.Response.CurrencyCode,
                From = dateRange.From,
                To = dateRange.To,
                Profit = totalProfit
            };

            return new QueryResponse<EntityProfitDto>
            {
                Status = QueryStatus.Ok,
                Response = profitDto
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.GetPortfolioPerformance"/>
        public async Task<QueryResponse<EntityPerformanceDto>> GetPortfolioPerformance(int portfolioId, DateRangeParams dateRange)
        {
            var portfolio = await GetPortfolio(portfolioId);
            if (portfolio.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityPerformanceDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            List<PortfolioTransactionDetailsQueryModel> transactions;
            using (var connection = _connectionCreator.CreateConnection())
            {
                transactions = (await GetTransactionsConvertedToPortfolioCurrency(connection, portfolioId, dateRange.SetFrom(PortEvalConstants.FinancialDataStartTime), dateRange)).ToList();
            }

            var performance = InternalRateOfReturnCalculator.CalculateIrr(transactions, dateRange.From, dateRange.To);

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

        /// <inheritdoc cref="IPortfolioQueries.ChartPortfolioValue"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioValue(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
        {
            var portfolio = await GetPortfolio(portfolioId);
            if (portfolio.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstTransactionTime = await GetFirstTransactionTime(portfolioId);
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
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(portfolio.Response.CurrencyCode,
                    targetCurrencyCode, EntityChartPointDto.FromValue((await GetPortfolioValue(portfolioId, range.To)).Response))
            );

            calculations = calculations.Prepend(await _exchangeRateQueries.ConvertChartPointCurrency(portfolio.Response.CurrencyCode,
                targetCurrencyCode, EntityChartPointDto.FromValue((await GetPortfolioValue(portfolioId, dateRange.From)).Response)));

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.ChartPortfolioProfit"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioProfit(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
        {
            var portfolio = await GetPortfolio(portfolioId);
            if (portfolio.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstTransactionTime = await GetFirstTransactionTime(portfolioId);
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
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(portfolio.Response.CurrencyCode,
                    targetCurrencyCode, EntityChartPointDto.FromProfit((await GetPortfolioProfit(portfolioId, range.SetFrom(dateRange.From))).Response))
            );

            calculations = calculations.Prepend(new EntityChartPointDto(dateRange.From, 0));

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.ChartPortfolioPerformance"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioPerformance(int portfolioId, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var portfolio = await GetPortfolio(portfolioId);
            if (portfolio.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstTransactionTime = await GetFirstTransactionTime(portfolioId);
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
                async range => EntityChartPointDto.FromPerformance((await GetPortfolioPerformance(portfolioId, range.SetFrom(dateRange.From))).Response)
            );

            calculations = calculations.Prepend(new EntityChartPointDto(dateRange.From, 0));
            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.ChartPortfolioProfitAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioProfitAggregated(int portfolioId,
            DateRangeParams dateRange, AggregationFrequency frequency, string targetCurrencyCode = null)
        {
            var portfolio = await GetPortfolio(portfolioId);
            if (portfolio.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstTransactionTime = await GetFirstTransactionTime(portfolioId);
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
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(portfolio.Response.CurrencyCode,
                    targetCurrencyCode, EntityChartPointDto.FromProfit((await GetPortfolioProfit(portfolioId, range)).Response))
            );

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.ChartPortfolioPerformanceAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartPortfolioPerformanceAggregated(int portfolioId, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var portfolio = await GetPortfolio(portfolioId);
            if (portfolio.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstTransactionTime = await GetFirstTransactionTime(portfolioId);
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
                async range => EntityChartPointDto.FromPerformance((await GetPortfolioPerformance(portfolioId, range)).Response)
            );

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.GetAllPortfoliosStatistics"/>
        public async Task<QueryResponse<IEnumerable<EntityStatisticsDto>>> GetAllPortfoliosStatistics()
        {
            var now = DateTime.UtcNow;

            var portfolios = await GetPortfolios();

            var data = await Task.WhenAll(portfolios.Response.Select(portfolio => GetPortfolioStatistics(portfolio.Id)));
            return new QueryResponse<IEnumerable<EntityStatisticsDto>>
            {
                Status = QueryStatus.Ok,
                Response = data.Select(stats => stats.Response)
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.GetPortfolioStatistics"/>
        public async Task<QueryResponse<EntityStatisticsDto>> GetPortfolioStatistics(int id)
        {
            var now = DateTime.UtcNow;

            var performanceTotal = await GetPortfolioPerformance(id, new DateRangeParams { To = now });
            var performanceLastDay = await GetPortfolioPerformance(id, new DateRangeParams { From = now.AddDays(-1), To = now });
            var performanceLastWeek = await GetPortfolioPerformance(id, new DateRangeParams { From = now.AddDays(-7), To = now });
            var performanceLastMonth = await GetPortfolioPerformance(id, new DateRangeParams { From = now.AddMonths(-1), To = now });

            var profitTotal = await GetPortfolioProfit(id, new DateRangeParams { To = now });
            var profitLastDay = await GetPortfolioProfit(id, new DateRangeParams { From = now.AddDays(-1), To = now });
            var profitLastWeek = await GetPortfolioProfit(id, new DateRangeParams { From = now.AddDays(-7), To = now });
            var profitLastMonth = await GetPortfolioProfit(id, new DateRangeParams { From = now.AddMonths(-1), To = now });

            if(profitLastMonth.Status != QueryStatus.Ok)
            {
                return new QueryResponse<EntityStatisticsDto>
                {
                    Status = profitLastMonth.Status
                };
            }

            return new QueryResponse<EntityStatisticsDto>
            {
                Status = QueryStatus.Ok,
                Response = new EntityStatisticsDto
                {
                    Id = id,
                    TotalPerformance = performanceTotal.Response?.Performance ?? 0m,
                    LastDayPerformance = performanceLastDay.Response?.Performance ?? 0m,
                    LastWeekPerformance = performanceLastWeek.Response?.Performance ?? 0m,
                    LastMonthPerformance = performanceLastMonth.Response?.Performance ?? 0m,
                    TotalProfit = profitTotal.Response?.Profit ?? 0m,
                    LastDayProfit = profitLastDay.Response?.Profit ?? 0m,
                    LastWeekProfit = profitLastWeek.Response?.Profit ?? 0m,
                    LastMonthProfit = profitLastMonth.Response?.Profit ?? 0m
                }
            };
        }

        /// <summary>
        /// Retrieves the time of the first available transaction of the given portfolio.
        /// </summary>
        /// <param name="portfolioId">Portfolio ID.</param>
        /// <returns>A task representing the asynchronous database query operation. Task result contains the time of the first transaction if such exists, null otherwise.</returns>
        private async Task<DateTime?> GetFirstTransactionTime(int portfolioId)
        {
            var query = PortfolioDataQueries.GetPortfolioFirstTransaction(portfolioId);

            using var connection = _connectionCreator.CreateConnection();
            var time = await connection.QueryFirstOrDefaultAsync<SingleTimeQueryModel>(query.Query, query.Params);
            return time.Time;
        }

        /// <summary>
        /// Retrieves transactions of the given portfolio in the specified date range converted to the portfolio's currency.
        /// </summary>
        /// <param name="connection">DbConnection to use for querying.</param>
        /// <param name="portfolioId">Portfolio ID.</param>
        /// <param name="transactionDateRange">Date range to retrieve transactions in.</param>
        /// <param name="instrumentPricesDateRange">Date range to use for instrument price calculations.</param>
        /// <returns>
        /// A task representing the asynchronous database query operation.
        /// Task result contains an <c>IEnumerable</c> of <see cref="PortfolioTransactionDetailsQueryModel">PortfolioTransactionDetailsQueryModel</see>
        /// with prices converted to portfolio's currency.
        /// </returns>
        private async Task<IEnumerable<PortfolioTransactionDetailsQueryModel>> GetTransactionsConvertedToPortfolioCurrency(IDbConnection connection,
            int portfolioId, DateRangeParams transactionDateRange, DateRangeParams instrumentPricesDateRange = null)
        {
            var firstTransactionTime = await GetFirstTransactionTime(portfolioId);

            if (firstTransactionTime == null)
            {
                return Enumerable.Empty<PortfolioTransactionDetailsQueryModel>();
            }

            var transactionsTimeFrom = transactionDateRange.From;
            var transactionsTimeTo = transactionDateRange.To;
            var pricesTimeFrom = instrumentPricesDateRange?.From ?? transactionsTimeFrom;
            var pricesTimeTo = instrumentPricesDateRange?.To ?? new DateTime(Math.Max(transactionsTimeFrom.Ticks, ((DateTime)firstTransactionTime).Ticks));
            var transactionQuery = PortfolioDataQueries.GetPortfolioDetailedTransactions(
                portfolioId,
                transactionsTimeFrom,
                transactionsTimeTo,
                pricesTimeFrom,
                pricesTimeTo 
            );
            var transactions =
                await connection.QueryAsync<PortfolioTransactionDetailsQueryModel>(transactionQuery.Query, transactionQuery.Params);

            var instrumentConvertedRangeStartPrices = new Dictionary<int, decimal>();
            var instrumentConvertedRangeEndPrices = new Dictionary<int, decimal>();
            foreach (var transaction in transactions)
            {
                if (transaction.TransactionCurrency == transaction.PortfolioCurrency) continue;

                var convertedTransactionPrice = await _exchangeRateQueries.Convert(transaction.TransactionCurrency,
                    transaction.PortfolioCurrency, transaction.Price, transaction.Time);

                if (!instrumentConvertedRangeStartPrices.ContainsKey(transaction.InstrumentId))
                {
                    instrumentConvertedRangeStartPrices[transaction.InstrumentId] = await _exchangeRateQueries.Convert(
                        transaction.TransactionCurrency, transaction.PortfolioCurrency,
                        transaction.InstrumentPriceAtRangeStart, pricesTimeFrom);
                }

                if (!instrumentConvertedRangeEndPrices.ContainsKey(transaction.InstrumentId))
                {
                    instrumentConvertedRangeEndPrices[transaction.InstrumentId] = await _exchangeRateQueries.Convert(
                        transaction.TransactionCurrency, transaction.PortfolioCurrency,
                        transaction.InstrumentPriceAtRangeEnd, pricesTimeTo);
                }

                transaction.Price = convertedTransactionPrice;
                transaction.InstrumentPriceAtRangeStart = instrumentConvertedRangeStartPrices[transaction.InstrumentId];
                transaction.InstrumentPriceAtRangeEnd = instrumentConvertedRangeEndPrices[transaction.InstrumentId];
            }

            return transactions;
        }
    }
}
