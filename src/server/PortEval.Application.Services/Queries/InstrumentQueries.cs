using Dapper;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Queries.DataQueries;
using PortEval.Application.Services.Queries.Helpers;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Application.Services.Queries.Models;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Services.Extensions;

namespace PortEval.Application.Services.Queries
{
    /// <inheritdoc cref="IInstrumentQueries"/>
    public class InstrumentQueries : IInstrumentQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;
        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;

        public InstrumentQueries(IDbConnectionCreator connection, ICurrencyExchangeRateQueries exchangeRateQueries)
        {
            _connectionCreator = connection;
            _exchangeRateQueries = exchangeRateQueries;
        }

        /// <inheritdoc cref="IInstrumentQueries.GetAllInstruments"/>
        public async Task<QueryResponse<IEnumerable<InstrumentDto>>> GetAllInstruments()
        {
            var query = InstrumentDataQueries.GetAllInstrumentsQuery();
            using var connection = _connectionCreator.CreateConnection();
            var instruments = await connection.QueryAsync<InstrumentDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<InstrumentDto>>
            {
                Status = QueryStatus.Ok,
                Response = instruments
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentsPage"/>
        public async Task<QueryResponse<PaginatedResponse<InstrumentDto>>> GetInstrumentsPage(PaginationParams pagination)
        {
            var pageQuery = InstrumentDataQueries.GetInstrumentPageQuery(pagination);
            var totalCountQuery = InstrumentDataQueries.GetInstrumentCountQuery();

            using var connection = _connectionCreator.CreateConnection();

            var totalCount = await connection.QueryFirstAsync<int>(totalCountQuery.Query, totalCountQuery.Params);
            var data = (await connection.QueryAsync<InstrumentDto>(pageQuery.Query, pageQuery.Params)).ToList();

            var response = new PaginatedResponse<InstrumentDto>(pagination.Page, data.Count, totalCount, data);

            return new QueryResponse<PaginatedResponse<InstrumentDto>>
            {
                Status = QueryStatus.Ok,
                Response = response
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrument"/>
        public async Task<QueryResponse<InstrumentDto>> GetInstrument(int instrumentId)
        {
            var query = InstrumentDataQueries.GetInstrumentQuery(instrumentId);

            using var connection = _connectionCreator.CreateConnection();
            var instrument = await connection.QueryFirstOrDefaultAsync<InstrumentDto>(query.Query, query.Params);

            return new QueryResponse<InstrumentDto>
            {
                Status = instrument != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = instrument
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetKnownExchanges"/>
        public async Task<QueryResponse<IEnumerable<ExchangeDto>>> GetKnownExchanges()
        {
            var query = InstrumentDataQueries.GetKnownExchangesQuery();

            using var connection = _connectionCreator.CreateConnection();
            var exchanges = await connection.QueryAsync<InstrumentExchangeQueryModel>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<ExchangeDto>>
            {
                Status = QueryStatus.Ok,
                Response = exchanges.Select(e => new ExchangeDto { Name = e.Exchange })
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPrices"/>
        public async Task<QueryResponse<IEnumerable<InstrumentPriceDto>>> GetInstrumentPrices(int instrumentId,
            DateRangeParams dateRange)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<InstrumentPriceDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var query = InstrumentDataQueries.GetInstrumentPrices(instrumentId, dateRange.From, dateRange.To);

            using var connection = _connectionCreator.CreateConnection();
            var prices = await connection.QueryAsync<InstrumentPriceDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<InstrumentPriceDto>>
            {
                Status = QueryStatus.Ok,
                Response = prices
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPricesPage"/>
        public async Task<QueryResponse<PaginatedResponse<InstrumentPriceDto>>> GetInstrumentPricesPage(int instrumentId,
            PaginationParams pagination, DateRangeParams dateRange, AggregationFrequency? frequency = null)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<PaginatedResponse<InstrumentPriceDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var pricePageQuery =
                InstrumentDataQueries.GetInstrumentPricesPage(instrumentId, dateRange.From, dateRange.To, pagination, frequency);
            var totalCountQuery =
                InstrumentDataQueries.GetInstrumentPriceCount(instrumentId, dateRange.From, dateRange.To, frequency);

            using var connection = _connectionCreator.CreateConnection();
            var totalCount = await connection.QueryFirstAsync<int>(totalCountQuery.Query, totalCountQuery.Params);
            var data = (await connection.QueryAsync<InstrumentPriceDto>(pricePageQuery.Query, pricePageQuery.Params))
                .ToList();

            var response = new PaginatedResponse<InstrumentPriceDto>(pagination.Page, data.Count, totalCount, data);

            return new QueryResponse<PaginatedResponse<InstrumentPriceDto>>
            {
                Status = QueryStatus.Ok,
                Response = response
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPrice"/>
        public async Task<QueryResponse<InstrumentPriceDto>> GetInstrumentPrice(int instrumentId, DateTime time)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<InstrumentPriceDto>
                {
                    Status = QueryStatus.NotFound
                };
            }
            var query = InstrumentDataQueries.GetInstrumentPrice(instrumentId, time);

            using var connection = _connectionCreator.CreateConnection();
            var price = await connection.QueryFirstOrDefaultAsync<InstrumentPriceDto>(query.Query, query.Params);

            return new QueryResponse<InstrumentPriceDto>
            {
                Status = QueryStatus.Ok,
                Response = price
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentProfit"/>
        public async Task<QueryResponse<EntityProfitDto>> GetInstrumentProfit(int instrumentId, DateRangeParams dateRange)
        {
            var instrument = await GetInstrument(instrumentId);
            if(instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityProfitDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstPrice = (await GetInstrumentPrice(instrumentId, dateRange.From)).Response?.Price ?? 0;
            var secondPrice = (await GetInstrumentPrice(instrumentId, dateRange.To)).Response?.Price ?? 0;

            var profit = new EntityProfitDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Profit = secondPrice - firstPrice
            };
            return new QueryResponse<EntityProfitDto>
            {
                Status = QueryStatus.Ok,
                Response = profit
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPerformance"/>
        public async Task<QueryResponse<EntityPerformanceDto>> GetInstrumentPerformance(int instrumentId, DateRangeParams dateRange)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityPerformanceDto>
                {
                    Status = QueryStatus.NotFound
                };
            }
            var firstPrice = (await GetInstrumentPrice(instrumentId, dateRange.From)).Response?.Price ?? 0;
            var secondPrice = (await GetInstrumentPrice(instrumentId, dateRange.To)).Response?.Price ?? 0;

            var calculatedPerformance = 0m;
            if(firstPrice == 0 && secondPrice > 0)
            {
                calculatedPerformance = 1;
            }
            else if (firstPrice == 0 && secondPrice == 0)
            {
                calculatedPerformance = 0;
            }
            else
            {
                calculatedPerformance = (secondPrice - firstPrice) / firstPrice;
            }

            var performance = new EntityPerformanceDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Performance = calculatedPerformance
            };

            return new QueryResponse<EntityPerformanceDto>
            {
                Status = QueryStatus.Ok,
                Response = performance
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentPrices"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPrices(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrument = await GetInstrument(instrumentId);
            if(instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstPriceTime = await GetFirstPriceTime(instrumentId);
            if (firstPriceTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }

            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstPriceTime));

            var process = new Func<DateTime, Task<EntityChartPointDto>>(async time =>
            {
                var price = await GetInstrumentPrice(instrumentId, time);
                if (price?.Response == null || price.Status != QueryStatus.Ok) return new EntityChartPointDto(time, 0m);

                price.Response.Time = time;
                return await _exchangeRateQueries.ConvertChartPointCurrency(instrument.Response.CurrencyCode, currencyCode,
                    EntityChartPointDto.FromInstrumentPrice(price.Response));
            });

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => await process(range.To)
            );

            var startPrice = await process(dateRange.From);
            if(startPrice != null)
            {
                calculations = calculations.Prepend(startPrice);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentProfit"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfit(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstPriceTime = await GetFirstPriceTime(instrumentId);
            if (firstPriceTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }

            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstPriceTime));

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(instrument.Response.CurrencyCode, currencyCode,
                    EntityChartPointDto.FromProfit((await GetInstrumentProfit(instrumentId, range.SetFrom(dateRange.From))).Response))
            );

            calculations = calculations.Prepend(new EntityChartPointDto(dateRange.From, 0));

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentPerformance"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformance(int instrumentId, DateRangeParams dateRange,
            AggregationFrequency frequency)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstPriceTime = await GetFirstPriceTime(instrumentId);
            if (firstPriceTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }

            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstPriceTime));

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => EntityChartPointDto.FromPerformance((await GetInstrumentPerformance(instrumentId, range.SetFrom(dateRange.From))).Response)
            );

            calculations = calculations.Prepend(new EntityChartPointDto(dateRange.From, 0));

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentProfitAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfitAggregated(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstPriceTime = await GetFirstPriceTime(instrumentId);
            if (firstPriceTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }

            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstPriceTime));

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(instrument.Response.CurrencyCode, currencyCode,
                    EntityChartPointDto.FromProfit((await GetInstrumentProfit(instrumentId, range)).Response))
            );

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentPerformanceAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformanceAggregated(int instrumentId, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstPriceTime = await GetFirstPriceTime(instrumentId);
            if (firstPriceTime == null)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.Ok,
                    Response = Enumerable.Empty<EntityChartPointDto>()
                };
            }

            dateRange = dateRange.SetFrom(dateRange.From.GetMax((DateTime)firstPriceTime));

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => EntityChartPointDto.FromPerformance((await GetInstrumentPerformance(instrumentId, range)).Response)
            );

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        private async Task<DateTime?> GetFirstPriceTime(int instrumentId)
        {
            var query = InstrumentDataQueries.GetFirstPriceTime(instrumentId);
            using var connection = _connectionCreator.CreateConnection();
            var timeModel = await connection.QueryFirstOrDefaultAsync<SingleTimeQueryModel>(query.Query, query.Params);

            return timeModel?.Time;
        }
    }
}
