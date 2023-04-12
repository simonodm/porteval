using PortEval.Application.Core.Interfaces;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher.Interfaces;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.FinancialDataFetcher
{
    /// <inheritdoc />
    public class FinancialDataFetcher : IFinancialDataFetcher
    {
        private readonly IDataFetcher _fetcher;

        public FinancialDataFetcher(IDataFetcher fetcher)
        {
            _fetcher = fetcher;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PricePoint>> GetHistoricalDailyPricesAsync(string symbol, string currencyCode, DateTime from,
            DateTime to)
        {
            var request = new HistoricalDailyInstrumentPricesRequest
            {
                Symbol = symbol,
                From = from,
                To = to
            };

            var response = await _fetcher.ProcessRequestAsync<HistoricalDailyInstrumentPricesRequest, IEnumerable<PricePoint>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PricePoint>> GetIntradayPricesAsync(string symbol, string currencyCode, DateTime from, DateTime to,
            IntradayInterval interval = IntradayInterval.OneHour)
        {
            var request = new IntradayInstrumentPricesRequest
            {
                Symbol = symbol,
                From = from,
                To = to,
                Interval = interval
            };

            var response = await _fetcher.ProcessRequestAsync<IntradayInstrumentPricesRequest, IEnumerable<PricePoint>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<PricePoint> GetLatestInstrumentPriceAsync(string symbol, string currencyCode)
        {
            var request = new LatestInstrumentPriceRequest
            {
                Symbol = symbol
            };

            var response = await _fetcher.ProcessRequestAsync<LatestInstrumentPriceRequest, PricePoint>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PricePoint>> GetHistoricalDailyCryptoPricesAsync(string symbol, string targetCurrency,
            DateTime from, DateTime to)
        {
            var request = new HistoricalDailyCryptoPricesRequest
            {
                Symbol = symbol,
                CurrencyCode = targetCurrency,
                From = from,
                To = to
            };

            var response = await _fetcher.ProcessRequestAsync<HistoricalDailyCryptoPricesRequest, IEnumerable<PricePoint>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PricePoint>> GetIntradayCryptoPricesAsync(string symbol, string targetCurrency,
            DateTime from, DateTime to, IntradayInterval interval = IntradayInterval.OneHour)
        {
            var request = new IntradayCryptoPricesRequest
            {
                Symbol = symbol,
                CurrencyCode = targetCurrency,
                From = from,
                To = to,
                Interval = interval
            };

            var response = await _fetcher.ProcessRequestAsync<IntradayCryptoPricesRequest, IEnumerable<PricePoint>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<PricePoint> GetLatestCryptoPriceAsync(string symbol, string targetCurrency)
        {
            var request = new LatestCryptoPriceRequest
            {
                Symbol = symbol,
                CurrencyCode = targetCurrency
            };

            var response = await _fetcher.ProcessRequestAsync<LatestCryptoPriceRequest, PricePoint>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<ExchangeRates> GetLatestExchangeRatesAsync(string baseCurrency)
        {
            var request = new LatestExchangeRatesRequest
            {
                CurrencyCode = baseCurrency
            };

            var response = await _fetcher.ProcessRequestAsync<LatestExchangeRatesRequest, ExchangeRates>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ExchangeRates>> GetHistoricalDailyExchangeRatesAsync(string baseCurrency, DateTime from, DateTime to)
        {
            var request = new HistoricalDailyExchangeRatesRequest
            {
                CurrencyCode = baseCurrency,
                From = from,
                To = to
            };

            var response = await _fetcher.ProcessRequestAsync<HistoricalDailyExchangeRatesRequest, IEnumerable<ExchangeRates>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InstrumentSplitData>> GetInstrumentSplitsAsync(string symbol, DateTime from,
            DateTime to)
        {
            var request = new InstrumentSplitsRequest
            {
                Symbol = symbol,
                From = from,
                To = to
            };

            var response = await _fetcher.ProcessRequestAsync<InstrumentSplitsRequest,
                IEnumerable<InstrumentSplitData>>(request);
            return response.Result;
        }
    }
}
