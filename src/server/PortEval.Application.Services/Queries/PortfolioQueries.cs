using Dapper;
using PortEval.Application.Features.Extensions;
using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Queries.DataQueries;
using PortEval.Application.Features.Queries.Helpers;
using PortEval.Application.Features.Queries.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.Interfaces.ChartDataGenerators;

namespace PortEval.Application.Features.Queries
{
    /// <inheritdoc cref="IPortfolioQueries"/>
    public class PortfolioQueries : IPortfolioQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;
        private readonly IPositionQueries _positionQueries;
        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;

        private readonly IPositionValueCalculator _valueCalculator;
        private readonly IPositionProfitCalculator _profitCalculator;
        private readonly IPositionPerformanceCalculator _performanceCalculator;

        private readonly IPositionChartDataGenerator _chartGenerator;
        private readonly ICurrencyConverter _currencyConverter;

        public PortfolioQueries(IDbConnectionCreator connection, ICurrencyExchangeRateQueries exchangeRateQueries,
            IPositionValueCalculator valueCalculator, IPositionProfitCalculator profitCalculator,
            IPositionPerformanceCalculator performanceCalculator, IPositionChartDataGenerator chartGenerator, ICurrencyConverter currencyConverter,
            IPositionQueries positionQueries)
        {
            _connectionCreator = connection;
            _exchangeRateQueries = exchangeRateQueries;
            _valueCalculator = valueCalculator;
            _profitCalculator = profitCalculator;
            _performanceCalculator = performanceCalculator;
            _chartGenerator = chartGenerator;
            _currencyConverter = currencyConverter;
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
            if (portfolio.Status == QueryStatus.NotFound)
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

            var positionsPriceData = await GetPositionsPriceData(portfolio.Response, dateRange);
            var value = _valueCalculator.CalculateValue(positionsPriceData, time);

            var valueDto = new EntityValueDto
            {
                Time = time,
                Value = value,
                CurrencyCode = portfolio.Response.CurrencyCode
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

            var positionsPriceData = await GetPositionsPriceRangeData(portfolio.Response, dateRange);
            var profit = _profitCalculator.CalculateProfit(positionsPriceData, dateRange.From, dateRange.To);

            var profitDto = new EntityProfitDto
            {
                CurrencyCode = portfolio.Response.CurrencyCode,
                From = dateRange.From,
                To = dateRange.To,
                Profit = profit
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

            var positionsPriceData = await GetPositionsPriceRangeData(portfolio.Response, dateRange);
            var performance = _performanceCalculator.CalculatePerformance(positionsPriceData, dateRange.From, dateRange.To);

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

            var positionsPriceData = await GetPositionsPriceListData(portfolio.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartValue(positionsPriceData, dateRange, frequency);
            if (!string.IsNullOrWhiteSpace(targetCurrencyCode) && portfolio.Response.CurrencyCode != targetCurrencyCode)
            {
                var exchangeRatesResponse =
                    await _exchangeRateQueries.GetExchangeRates(portfolio.Response.CurrencyCode, targetCurrencyCode, dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
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

            var positionsPriceData = await GetPositionsPriceListData(portfolio.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartProfit(positionsPriceData, dateRange, frequency);
            if (!string.IsNullOrWhiteSpace(targetCurrencyCode) && portfolio.Response.CurrencyCode != targetCurrencyCode)
            {
                var exchangeRatesResponse =
                    await _exchangeRateQueries.GetExchangeRates(portfolio.Response.CurrencyCode, targetCurrencyCode, dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
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

            var positionsPriceData = await GetPositionsPriceListData(portfolio.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartPerformance(positionsPriceData, dateRange, frequency);

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
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

            var positionsPriceData = await GetPositionsPriceListData(portfolio.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartAggregatedProfit(positionsPriceData, dateRange, frequency);
            if (!string.IsNullOrWhiteSpace(targetCurrencyCode) && portfolio.Response.CurrencyCode != targetCurrencyCode)
            {
                var exchangeRatesResponse =
                    await _exchangeRateQueries.GetExchangeRates(portfolio.Response.CurrencyCode, targetCurrencyCode, dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
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

            var positionsPriceData = await GetPositionsPriceListData(portfolio.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartAggregatedPerformance(positionsPriceData, dateRange, frequency);

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.GetAllPortfoliosStatistics"/>
        public async Task<QueryResponse<IEnumerable<EntityStatisticsDto>>> GetAllPortfoliosStatistics()
        {
            var portfolios = await GetPortfolios();

            var data = await Task.WhenAll(portfolios.Response.Select(GetPortfolioStatistics));
            return new QueryResponse<IEnumerable<EntityStatisticsDto>>
            {
                Status = QueryStatus.Ok,
                Response = data.Select(stats => stats.Response)
            };
        }

        /// <inheritdoc cref="IPortfolioQueries.GetPortfolioStatistics"/>
        public async Task<QueryResponse<EntityStatisticsDto>> GetPortfolioStatistics(int id)
        {
            var portfolio = await GetPortfolio(id);
            if (portfolio.Status != QueryStatus.Ok)
            {
                return new QueryResponse<EntityStatisticsDto>
                {
                    Status = portfolio.Status
                };
            }

            return await GetPortfolioStatistics(portfolio.Response);
        }

        private async Task<QueryResponse<EntityStatisticsDto>> GetPortfolioStatistics(PortfolioDto portfolio)
        {
            var now = DateTime.UtcNow;

            var positionsTotalPriceRangeData = await GetPositionsPriceRangeData(portfolio, new DateRangeParams { To = now });
            var positionsLastDayPriceRangeData = await GetPositionsPriceRangeData(portfolio, new DateRangeParams { From = now.AddDays(-1), To = now });
            var positionsLastWeekPriceRangeData = await GetPositionsPriceRangeData(portfolio, new DateRangeParams { From = now.AddDays(-7), To = now });
            var positionsLastMonthPriceRangeData = await GetPositionsPriceRangeData(portfolio, new DateRangeParams { From = now.AddMonths(-1), To = now });

            var performanceTotal =
                _performanceCalculator.CalculatePerformance(positionsTotalPriceRangeData, DateTime.MinValue, now);
            var performanceLastDay = _performanceCalculator.CalculatePerformance(positionsLastDayPriceRangeData, now.AddDays(-1), now);
            var performanceLastWeek = _performanceCalculator.CalculatePerformance(positionsLastWeekPriceRangeData, now.AddDays(-7), now);
            var performanceLastMonth = _performanceCalculator.CalculatePerformance(positionsLastMonthPriceRangeData, now.AddMonths(-1), now);

            var profitTotal = _profitCalculator.CalculateProfit(positionsTotalPriceRangeData, DateTime.MinValue, now);
            var profitLastDay = _profitCalculator.CalculateProfit(positionsLastDayPriceRangeData, now.AddDays(-1), now);
            var profitLastWeek = _profitCalculator.CalculateProfit(positionsLastWeekPriceRangeData, now.AddDays(-7), now);
            var profitLastMonth = _profitCalculator.CalculateProfit(positionsLastMonthPriceRangeData, now.AddMonths(-1), now);

            return new QueryResponse<EntityStatisticsDto>
            {
                Status = QueryStatus.Ok,
                Response = new EntityStatisticsDto
                {
                    Id = portfolio.Id,
                    TotalPerformance = performanceTotal,
                    LastDayPerformance = performanceLastDay,
                    LastWeekPerformance = performanceLastWeek,
                    LastMonthPerformance = performanceLastMonth,
                    TotalProfit = profitTotal,
                    LastDayProfit = profitLastDay,
                    LastWeekProfit = profitLastWeek,
                    LastMonthProfit = profitLastMonth
                }
            };
        }

        private async Task<IEnumerable<PositionPriceData>> GetPositionsPriceData(PortfolioDto portfolio,
            DateRangeParams dateRange)
        {
            var positions = await _positionQueries.GetPortfolioPositions(portfolio.Id);

            var positionsPriceData = new List<PositionPriceData>();

            foreach (var position in positions.Response)
            {
                var priceData = await _positionQueries.GetPositionPriceData(position, dateRange);
                if (portfolio.CurrencyCode != position.Instrument.CurrencyCode)
                {
                    var exchangeRates = await _exchangeRateQueries.GetExchangeRates(position.Instrument.CurrencyCode,
                        portfolio.CurrencyCode, dateRange.SetFrom(DateTime.MinValue));

                    priceData.Transactions = _currencyConverter.ConvertTransactions(priceData.Transactions, exchangeRates.Response);
                    priceData.Price =
                        _currencyConverter.ConvertInstrumentPrices(new[] { priceData.Price }, exchangeRates.Response).First();
                }

                positionsPriceData.Add(priceData);
            }

            return positionsPriceData;
        }

        private async Task<IEnumerable<PositionPriceRangeData>> GetPositionsPriceRangeData(PortfolioDto portfolio,
            DateRangeParams dateRange)
        {
            var positions = await _positionQueries.GetPortfolioPositions(portfolio.Id);

            var positionsPriceData = new List<PositionPriceRangeData>();

            foreach (var position in positions.Response)
            {
                var priceData = await _positionQueries.GetPositionPriceRangeData(position, dateRange);
                if (portfolio.CurrencyCode != position.Instrument.CurrencyCode)
                {
                    var exchangeRates = await _exchangeRateQueries.GetExchangeRates(position.Instrument.CurrencyCode,
                        portfolio.CurrencyCode, dateRange.SetFrom(DateTime.MinValue));

                    priceData.Transactions = _currencyConverter.ConvertTransactions(priceData.Transactions, exchangeRates.Response);
                    priceData.PriceAtRangeStart =
                        _currencyConverter.ConvertInstrumentPrices(new[] { priceData.PriceAtRangeStart }, exchangeRates.Response).First();
                    priceData.PriceAtRangeEnd =
                        _currencyConverter.ConvertInstrumentPrices(new[] { priceData.PriceAtRangeEnd },
                            exchangeRates.Response).First();
                }

                positionsPriceData.Add(priceData);
            }

            return positionsPriceData;
        }

        private async Task<IEnumerable<PositionPriceListData>> GetPositionsPriceListData(PortfolioDto portfolio,
            DateRangeParams dateRange)
        {
            var positions = await _positionQueries.GetPortfolioPositions(portfolio.Id);

            var positionsPriceData = new List<PositionPriceListData>();

            foreach (var position in positions.Response)
            {
                var priceData = await _positionQueries.GetPositionPriceListData(position, dateRange);
                if (portfolio.CurrencyCode != position.Instrument.CurrencyCode)
                {
                    var exchangeRates = await _exchangeRateQueries.GetExchangeRates(position.Instrument.CurrencyCode,
                        portfolio.CurrencyCode, dateRange.SetFrom(DateTime.MinValue));

                    priceData.Transactions = _currencyConverter.ConvertTransactions(priceData.Transactions, exchangeRates.Response);
                    priceData.Prices =
                        _currencyConverter.ConvertInstrumentPrices(priceData.Prices, exchangeRates.Response);
                }

                positionsPriceData.Add(priceData);
            }

            return positionsPriceData;
        }
    }
}
