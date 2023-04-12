using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Services
{
    /// <inheritdoc cref="IInstrumentService"/>
    public class InstrumentService : IInstrumentService
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeRepository _exchangeRepository;
        private readonly IInstrumentQueries _instrumentDataQueries;
        private readonly IExchangeQueries _exchangeDataQueries;
        private readonly ICurrencyExchangeRateService _exchangeRateService;
        private readonly IInstrumentPriceService _priceService;
        private readonly IInstrumentProfitCalculator _profitCalculator;
        private readonly IInstrumentPerformanceCalculator _performanceCalculator;
        private readonly IInstrumentChartDataGenerator _chartDataGenerator;
        private readonly ICurrencyConverter _currencyConverter;

        public InstrumentService(IInstrumentRepository instrumentRepository, ICurrencyRepository currencyRepository, IExchangeRepository exchangeRepository, IInstrumentQueries instrumentDataQueries, IExchangeQueries exchangeDataQueries, IInstrumentPriceService priceService, IInstrumentProfitCalculator profitCalculator, IInstrumentPerformanceCalculator performanceCalculator, IInstrumentChartDataGenerator chartDataGenerator, ICurrencyQueries currencyDataQueries, ICurrencyExchangeRateService exchangeRateService, ICurrencyConverter currencyConverter)
        {
            _instrumentRepository = instrumentRepository;
            _currencyRepository = currencyRepository;
            _exchangeRepository = exchangeRepository;
            _instrumentDataQueries = instrumentDataQueries;
            _exchangeDataQueries = exchangeDataQueries;
            _priceService = priceService;
            _profitCalculator = profitCalculator;
            _performanceCalculator = performanceCalculator;
            _chartDataGenerator = chartDataGenerator;
            _exchangeRateService = exchangeRateService;
            _currencyConverter = currencyConverter;
        }

        /// <inheritdoc />
        public async Task<OperationResponse<IEnumerable<InstrumentDto>>> GetAllInstrumentsAsync()
        {
            var instruments = await _instrumentDataQueries.GetAllInstrumentsAsync();

            return new OperationResponse<IEnumerable<InstrumentDto>>
            {
                Response = instruments
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<PaginatedResponse<InstrumentDto>>> GetInstrumentsPageAsync(
            PaginationParams pagination)
        {
            var totalCount = await _instrumentDataQueries.GetInstrumentCountAsync();
            var data = await _instrumentDataQueries.GetInstrumentPageAsync(pagination);

            var page = data.ToList();
            return new OperationResponse<PaginatedResponse<InstrumentDto>>
            {
                Response = new PaginatedResponse<InstrumentDto>(pagination.Page, page.Count, totalCount, page)
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<InstrumentDto>> GetInstrumentAsync(int instrumentId)
        {
            var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);

            return new OperationResponse<InstrumentDto>
            {
                Status = instrument != null ? OperationStatus.Ok : OperationStatus.NotFound,
                Message = instrument != null ? "" : $"Instrument {instrumentId} does not exist.",
                Response = instrument
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<IEnumerable<ExchangeDto>>> GetKnownExchangesAsync()
        {
            var exchanges = await _exchangeDataQueries.GetKnownExchangesAsync();

            return new OperationResponse<IEnumerable<ExchangeDto>>
            {
                Response = exchanges
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<EntityProfitDto>> GetInstrumentProfitAsync(int instrumentId,
            DateRangeParams dateRange)
        {
            var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);
            if (instrument == null)
            {
                return new OperationResponse<EntityProfitDto>
                {
                    Status = OperationStatus.NotFound,
                    Message = $"Instrument {instrumentId} does not exist."
                };
            }

            var firstPrice =
                (await _priceService.GetInstrumentPriceAsync(instrumentId, dateRange.From)).Response?.Price ?? 0m;
            var secondPrice =
                (await _priceService.GetInstrumentPriceAsync(instrumentId, dateRange.To)).Response?.Price ?? 0m;

            var profit = _profitCalculator.CalculateProfit(firstPrice, secondPrice);

            return new OperationResponse<EntityProfitDto>
            {
                Status = OperationStatus.Ok,
                Response = new EntityProfitDto
                {
                    CurrencyCode = instrument.CurrencyCode,
                    From = dateRange.From,
                    To = dateRange.To,
                    Profit = profit
                }
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<EntityPerformanceDto>> GetInstrumentPerformanceAsync(int instrumentId,
            DateRangeParams dateRange)
        {
            var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);
            if (instrument == null)
            {
                return new OperationResponse<EntityPerformanceDto>
                {
                    Status = OperationStatus.NotFound,
                    Message = $"Instrument {instrumentId} does not exist."
                };
            }

            var firstPrice =
                (await _priceService.GetInstrumentPriceAsync(instrumentId, dateRange.From)).Response?.Price ?? 0m;
            var secondPrice =
                (await _priceService.GetInstrumentPriceAsync(instrumentId, dateRange.To)).Response?.Price ?? 0m;

            var performance = _performanceCalculator.CalculatePerformance(firstPrice, secondPrice);

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
        public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPricesAsync(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);
            if (instrument == null)
            {
                return new OperationResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = OperationStatus.NotFound,
                    Message = $"Instrument {instrumentId} does not exist."
                };
            }

            var prices =
                await _instrumentDataQueries.GetInstrumentPricesAsync(instrumentId, DateTime.MinValue, dateRange.To);
            var result = _chartDataGenerator.ChartPrices(prices, dateRange, frequency);

            if (!string.Equals(instrument.CurrencyCode, currencyCode, StringComparison.CurrentCultureIgnoreCase))
            {
                var exchangeRates = await _exchangeRateService.GetExchangeRatesAsync(instrument.CurrencyCode, currencyCode,
                    dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRates.Response);
            }

            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfitAsync(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);
            if (instrument == null)
            {
                return new OperationResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = OperationStatus.NotFound,
                    Message = $"Instrument {instrumentId} does not exist."
                };
            }

            var prices =
                await _instrumentDataQueries.GetInstrumentPricesAsync(instrumentId, DateTime.MinValue, dateRange.To);
            var result = _chartDataGenerator.ChartProfit(prices, dateRange, frequency);

            if (!string.Equals(instrument.CurrencyCode, currencyCode, StringComparison.CurrentCultureIgnoreCase))
            {
                var exchangeRates = await _exchangeRateService.GetExchangeRatesAsync(instrument.CurrencyCode, currencyCode,
                    dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRates.Response);
            }

            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformanceAsync(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);
            if (instrument == null)
            {
                return new OperationResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = OperationStatus.NotFound,
                    Message = $"Instrument {instrumentId} does not exist."
                };
            }

            var prices =
                await _instrumentDataQueries.GetInstrumentPricesAsync(instrumentId, DateTime.MinValue, dateRange.To);
            var result = _chartDataGenerator.ChartPerformance(prices, dateRange, frequency);

            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentAggregatedProfitAsync(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);
            if (instrument == null)
            {
                return new OperationResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = OperationStatus.NotFound,
                    Message = $"Instrument {instrumentId} does not exist."
                };
            }

            var prices =
                await _instrumentDataQueries.GetInstrumentPricesAsync(instrumentId, DateTime.MinValue, dateRange.To);
            var result = _chartDataGenerator.ChartAggregatedProfit(prices, dateRange, frequency);

            if (!string.Equals(instrument.CurrencyCode, currencyCode, StringComparison.CurrentCultureIgnoreCase))
            {
                var exchangeRates = await _exchangeRateService.GetExchangeRatesAsync(instrument.CurrencyCode, currencyCode,
                    dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRates.Response);
            }

            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentAggregatedPerformanceAsync(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var instrument = await _instrumentDataQueries.GetInstrumentAsync(instrumentId);
            if (instrument == null)
            {
                return new OperationResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = OperationStatus.NotFound,
                    Message = $"Instrument {instrumentId} does not exist."
                };
            }

            var prices =
                await _instrumentDataQueries.GetInstrumentPricesAsync(instrumentId, DateTime.MinValue, dateRange.To);
            var result = _chartDataGenerator.ChartAggregatedPerformance(prices, dateRange, frequency);

            return new OperationResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = OperationStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc />
        public async Task<OperationResponse<InstrumentDto>> CreateInstrumentAsync(InstrumentDto options)
        {
            if (!await _currencyRepository.ExistsAsync(options.CurrencyCode))
            {
                return new OperationResponse<InstrumentDto>
                {
                    Status = OperationStatus.Error,
                    Message = $"Currency {options.CurrencyCode} does not exist."
                };
            }

            if (await _instrumentRepository.ExistsAsync(options.Symbol))
            {
                return new OperationResponse<InstrumentDto>
                {
                    Status = OperationStatus.Error,
                    Message = $"An instrument with symbol {options.Symbol} already exists."
                };
            }

            await CreateExchangeIfDoesNotExistAsync(options.Exchange);

            var instrument = Instrument.Create(options.Name, options.Symbol, string.IsNullOrEmpty(options.Exchange) ? null : options.Exchange,
                options.Type, options.CurrencyCode, options.Note);
            var createdInstrument = _instrumentRepository.Add(instrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
            return await GetInstrumentAsync(createdInstrument.Id);
        }

        /// <inheritdoc />
        public async Task<OperationResponse<InstrumentDto>> UpdateInstrumentAsync(InstrumentDto options)
        {
            var existingInstrument = await _instrumentRepository.FindAsync(options.Id);
            if (existingInstrument == null)
            {
                return new OperationResponse<InstrumentDto>
                {
                    Status = OperationStatus.NotFound,
                    Message = $"Instrument {options.Id} does not exist."
                };
            }

            await CreateExchangeIfDoesNotExistAsync(options.Exchange);

            existingInstrument.Rename(options.Name);
            existingInstrument.SetExchange(string.IsNullOrEmpty(options.Exchange) ? null : options.Exchange);
            existingInstrument.SetNote(options.Note);
            existingInstrument.IncreaseVersion();
            _instrumentRepository.Update(existingInstrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
            return await GetInstrumentAsync(existingInstrument.Id);
        }

        /// <inheritdoc />
        public async Task<OperationResponse> DeleteAsync(int id)
        {
            if (!await _instrumentRepository.ExistsAsync(id))
            {
                return new OperationResponse
                {
                    Status = OperationStatus.NotFound,
                    Message = $"Instrument {id} does not exist."
                };
            }

            await _instrumentRepository.DeleteAsync(id);
            await _instrumentRepository.UnitOfWork.CommitAsync();

            return new OperationResponse();
        }

        private async Task CreateExchangeIfDoesNotExistAsync(string exchange)
        {
            if (!string.IsNullOrEmpty(exchange) && !await _exchangeRepository.ExistsAsync(exchange))
            {
                var newExchange = Exchange.Create(exchange);
                _exchangeRepository.Add(newExchange);
            }
        }
    }
}
