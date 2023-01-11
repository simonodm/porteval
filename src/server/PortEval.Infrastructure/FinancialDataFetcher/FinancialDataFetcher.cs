using PortEval.Application.Features.Interfaces;
using PortEval.Application.Models.PriceFetcher;
using PortEval.DataFetcher.Interfaces;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.FinancialDataFetcher
{
    public class FinancialDataFetcher : IFinancialDataFetcher
    {
        private readonly IDataFetcher _fetcher;

        public FinancialDataFetcher(IDataFetcher fetcher)
        {
            _fetcher = fetcher;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PricePoint>> GetHistoricalDailyPrices(string symbol, string currencyCode, DateTime from,
            DateTime to)
        {
            var request = new HistoricalDailyInstrumentPricesRequest
            {
                Symbol = symbol,
                From = from,
                To = to
            };

            var response = await _fetcher.ProcessRequest<HistoricalDailyInstrumentPricesRequest, IEnumerable<PricePoint>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PricePoint>> GetIntradayPrices(string symbol, string currencyCode, DateTime from, DateTime to,
            IntradayInterval interval = IntradayInterval.OneHour)
        {
            var request = new IntradayInstrumentPricesRequest
            {
                Symbol = symbol,
                From = from,
                To = to,
                Interval = interval
            };

            var response = await _fetcher.ProcessRequest<IntradayInstrumentPricesRequest, IEnumerable<PricePoint>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<PricePoint> GetLatestInstrumentPrice(string symbol, string currencyCode)
        {
            var request = new LatestInstrumentPriceRequest
            {
                Symbol = symbol
            };

            var response = await _fetcher.ProcessRequest<LatestInstrumentPriceRequest, PricePoint>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PricePoint>> GetHistoricalDailyCryptoPrices(string symbol, string targetCurrency,
            DateTime from, DateTime to)
        {
            var request = new HistoricalDailyCryptoPricesRequest
            {
                Symbol = symbol,
                CurrencyCode = targetCurrency,
                From = from,
                To = to
            };

            var response = await _fetcher.ProcessRequest<HistoricalDailyCryptoPricesRequest, IEnumerable<PricePoint>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PricePoint>> GetIntradayCryptoPrices(string symbol, string targetCurrency,
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

            var response = await _fetcher.ProcessRequest<IntradayCryptoPricesRequest, IEnumerable<PricePoint>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<PricePoint> GetLatestCryptoPrice(string symbol, string targetCurrency)
        {
            var request = new LatestCryptoPriceRequest
            {
                Symbol = symbol,
                CurrencyCode = targetCurrency
            };

            var response = await _fetcher.ProcessRequest<LatestCryptoPriceRequest, PricePoint>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<ExchangeRates> GetLatestExchangeRates(string baseCurrency)
        {
            var request = new LatestExchangeRatesRequest
            {
                CurrencyCode = baseCurrency
            };

            var response = await _fetcher.ProcessRequest<LatestExchangeRatesRequest, ExchangeRates>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ExchangeRates>> GetHistoricalDailyExchangeRates(string baseCurrency, DateTime from, DateTime to)
        {
            var request = new HistoricalDailyExchangeRatesRequest
            {
                CurrencyCode = baseCurrency,
                From = from,
                To = to
            };

            var response = await _fetcher.ProcessRequest<HistoricalDailyExchangeRatesRequest, IEnumerable<ExchangeRates>>(request);
            return response.Result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InstrumentSplitData>> GetInstrumentSplits(string symbol, DateTime from,
            DateTime to)
        {
            var request = new InstrumentSplitsRequest
            {
                Symbol = symbol,
                From = from,
                To = to
            };

            var response = await _fetcher.ProcessRequest<InstrumentSplitsRequest,
                IEnumerable<InstrumentSplitData>>(request);
            return response.Result;
        }
    }
}
