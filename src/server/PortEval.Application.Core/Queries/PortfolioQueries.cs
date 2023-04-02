using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Queries.DataQueries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Queries
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

        private readonly IPortfolioStatisticsCalculator _statisticsCalculator;

        private readonly IPortfolioChartDataGenerator _chartGenerator;
        private readonly ICurrencyConverter _currencyConverter;

        public PortfolioQueries(IDbConnectionCreator connection, ICurrencyExchangeRateQueries exchangeRateQueries,
            IPositionValueCalculator valueCalculator, IPositionProfitCalculator profitCalculator,
            IPositionPerformanceCalculator performanceCalculator, IPortfolioChartDataGenerator chartGenerator, ICurrencyConverter currencyConverter,
            IPositionQueries positionQueries, IPortfolioStatisticsCalculator statisticsCalculator)
        {
            _connectionCreator = connection;
            _exchangeRateQueries = exchangeRateQueries;
            _valueCalculator = valueCalculator;
            _profitCalculator = profitCalculator;
            _performanceCalculator = performanceCalculator;
            _chartGenerator = chartGenerator;
            _currencyConverter = currencyConverter;
            _positionQueries = positionQueries;
            _statisticsCalculator = statisticsCalculator;
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

            var portfolioPositionsPriceRangeData = await GetPositionsPriceRangeData(portfolio.Response, dateRange);
            var value = _valueCalculator.CalculateValue(portfolioPositionsPriceRangeData.PositionsPriceRangeData, time);

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

            var portfolioPositionsPriceRangeData = await GetPositionsPriceRangeData(portfolio.Response, dateRange);
            var profit = _profitCalculator.CalculateProfit(portfolioPositionsPriceRangeData.PositionsPriceRangeData, dateRange.From, dateRange.To);

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

            var portfolioPositionsPriceRangeData = await GetPositionsPriceRangeData(portfolio.Response, dateRange);
            var performance = _performanceCalculator.CalculatePerformance(portfolioPositionsPriceRangeData.PositionsPriceRangeData, dateRange.From, dateRange.To);

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
            if (!string.Equals(portfolio.Response.CurrencyCode, targetCurrencyCode, StringComparison.CurrentCultureIgnoreCase))
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
            if (!string.Equals(portfolio.Response.CurrencyCode, targetCurrencyCode, StringComparison.CurrentCultureIgnoreCase))
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
            if (!string.Equals(portfolio.Response.CurrencyCode, targetCurrencyCode, StringComparison.CurrentCultureIgnoreCase))
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

            var priceData = await GetPositionsPriceListData(portfolio, new DateRangeParams { To = now });
            var stats = _statisticsCalculator.CalculateStatistics(priceData, now);

            return new QueryResponse<EntityStatisticsDto>
            {
                Status = QueryStatus.Ok,
                Response = stats
            };
        }

        private async Task<PortfolioPositionsPriceRangeData> GetPositionsPriceRangeData(PortfolioDto portfolio,
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

                    priceData.PositionId = position.Id;
                    priceData.Transactions = _currencyConverter.ConvertTransactions(priceData.Transactions, exchangeRates.Response);
                    priceData.PriceAtRangeStart =
                        _currencyConverter.ConvertInstrumentPrices(new[] { priceData.PriceAtRangeStart }, exchangeRates.Response).First();
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
            var positions = await _positionQueries.GetPortfolioPositions(portfolio.Id);

            var positionsPriceListData = new List<PositionPriceListData>();

            foreach (var position in positions.Response)
            {
                var priceData = await _positionQueries.GetPositionPriceListData(position, dateRange);
                if (portfolio.CurrencyCode != position.Instrument.CurrencyCode)
                {
                    var exchangeRates = await _exchangeRateQueries.GetExchangeRates(position.Instrument.CurrencyCode,
                        portfolio.CurrencyCode, dateRange.SetFrom(DateTime.MinValue));

                    priceData.PositionId = position.Id;
                    priceData.Transactions = _currencyConverter.ConvertTransactions(priceData.Transactions, exchangeRates.Response);
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
}
