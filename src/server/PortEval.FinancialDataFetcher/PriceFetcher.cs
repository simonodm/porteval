using PortEval.FinancialDataFetcher.APIs.ExchangeRateHost;
using PortEval.FinancialDataFetcher.APIs.OpenExchangeRates;
using PortEval.FinancialDataFetcher.APIs.RapidAPIMboum;
using PortEval.FinancialDataFetcher.APIs.Tiingo;
using PortEval.FinancialDataFetcher.Interfaces;
using PortEval.FinancialDataFetcher.Interfaces.APIs;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher
{
    /// <inheritdoc cref="IPriceFetcher"/>
    public class PriceFetcher : IPriceFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly List<IFinancialApi> _registeredClients = new List<IFinancialApi>();

        /// <summary>
        /// Initializes the price fetcher.
        /// </summary>
        public PriceFetcher()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Adds OpenExchangeRates to available APIs.
        /// </summary>
        /// <param name="apiKey">OpenExchangeRates API key</param>
        /// <param name="rateLimiter">Request rate limiter</param>
        public void AddOpenExchangeRates(string apiKey, RateLimiter rateLimiter = null)
        {
            var client = new OpenExchangeRatesApi(_httpClient, apiKey, rateLimiter);
            _registeredClients.Add(client);
        }

        /// <summary>
        /// Adds ExchangeRate.host to available APIs.
        /// </summary>
        /// <param name="rateLimiter">Request rate limiter</param>
        public void AddExchangeRateHost(RateLimiter rateLimiter = null)
        {
            var client = new ExchangeRateHostApi(_httpClient, rateLimiter);
            _registeredClients.Add(client);
        }

        /// <summary>
        /// Adds Tiingo to available APIs.
        /// </summary>
        /// <param name="apiKey">Tiingo API key</param>
        /// <param name="rateLimiter">Request rate limiter</param>
        public void AddTiingo(string apiKey, RateLimiter rateLimiter = null)
        {
            var client = new TiingoApi(_httpClient, apiKey, rateLimiter);
            _registeredClients.Add(client);
        }

        /// <summary>
        /// Adds RapidAPI's Mboum API to available APIs.
        /// </summary>
        /// <param name="apiKey">RapidAPI Mboum API key</param>
        /// <param name="rateLimiter">Request rate limiter</param>
        public void AddMboum(string apiKey, RateLimiter rateLimiter = null)
        {
            var client = new MboumApi(_httpClient, apiKey, rateLimiter);
            _registeredClients.Add(client);
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<PricePoint>>> GetHistoricalDailyPrices(string symbol, string currencyCode, DateTime from,
            DateTime to)
        {
            var request = new HistoricalDailyInstrumentPricesRequest
            {
                Symbol = symbol,
                From = from,
                To = to
            };

            return await ProcessRequest<IHistoricalDailyInstrumentPricesFinancialApi, HistoricalDailyInstrumentPricesRequest,
                IEnumerable<PricePoint>>(request);
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<PricePoint>>> GetIntradayPrices(string symbol, string currencyCode, DateTime from, DateTime to,
            IntradayInterval interval = IntradayInterval.OneHour)
        {
            var request = new IntradayInstrumentPricesRequest
            {
                Symbol = symbol,
                From = from,
                To = to,
                Interval = interval
            };

            return await ProcessRequest<IIntradayFinancialApi, IntradayInstrumentPricesRequest, IEnumerable<PricePoint>>(request);
        }

        /// <inheritdoc />
        public async Task<Response<PricePoint>> GetLatestInstrumentPrice(string symbol, string currencyCode)
        {
            var request = new LatestInstrumentPriceRequest
            {
                Symbol = symbol
            };

            return await ProcessRequest<ILatestInstrumentPriceFinancialApi, LatestInstrumentPriceRequest, PricePoint>(request);
        }
        
        /// <inheritdoc />
        public async Task<Response<IEnumerable<PricePoint>>> GetHistoricalDailyCryptoPrices(string symbol, string targetCurrency,
            DateTime from, DateTime to)
        {
            var request = new HistoricalDailyCryptoPricesRequest
            {
                Symbol = symbol,
                CurrencyCode = targetCurrency,
                From = from,
                To = to
            };

            return await ProcessRequest<IHistoricalDailyCryptoFinancialApi, HistoricalDailyCryptoPricesRequest,
                IEnumerable<PricePoint>>(request);
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<PricePoint>>> GetIntradayCryptoPrices(string symbol, string targetCurrency,
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

            return await ProcessRequest<IIntradayCryptoFinancialApi, IntradayCryptoPricesRequest, IEnumerable<PricePoint>>(request);
        }

        /// <inheritdoc />
        public async Task<Response<PricePoint>> GetLatestCryptoPrice(string symbol, string targetCurrency)
        {
            var request = new LatestCryptoPriceRequest
            {
                Symbol = symbol,
                CurrencyCode = targetCurrency
            };

            return await ProcessRequest<ILatestCryptoPriceFinancialApi, LatestCryptoPriceRequest, PricePoint>(request);
        }

        /// <inheritdoc />
        public async Task<Response<ExchangeRates>> GetLatestExchangeRates(string baseCurrency)
        {
            var request = new LatestExchangeRatesRequest
            {
                CurrencyCode = baseCurrency
            };

            return await ProcessRequest<ILatestExchangeRatesFinancialApi, LatestExchangeRatesRequest,
                ExchangeRates>(request);
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<ExchangeRates>>> GetHistoricalDailyExchangeRates(string baseCurrency, DateTime from, DateTime to)
        {
            var request = new HistoricalDailyExchangeRatesRequest
            {
                CurrencyCode = baseCurrency,
                From = from,
                To = to
            };

            return await ProcessRequest<IHistoricalDailyExchangeRatesFinancialApi,
                HistoricalDailyExchangeRatesRequest, IEnumerable<ExchangeRates>>(request);
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<InstrumentSplitData>>> GetInstrumentSplits(string symbol, DateTime from,
            DateTime to)
        {
            var request = new InstrumentSplitsRequest
            {
                Symbol = symbol,
                From = from,
                To = to
            };

            return await ProcessRequest<IInstrumentSplitFinancialApi, InstrumentSplitsRequest,
                IEnumerable<InstrumentSplitData>>(request);
        }

        /// <inheritdoc />
        public async Task<Response<TResult>> ProcessRequest<TClient, TRequest, TResult>(TRequest request)
            where TClient : class, IFinancialApi<TRequest, Response<TResult>>
            where TRequest : IRequest
        {
            var eligibleApis = new List<TClient>();
            foreach (var client in _registeredClients)
            {
                if (client is TClient targetClient)
                {
                    eligibleApis.Add(targetClient);
                }
            }

            if (eligibleApis.Count == 0)
            {
                return new Response<TResult>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "No eligible API found for the given request."
                };
            }

            var handler = new RequestHandler<TClient, TRequest, Response<TResult>>(request, eligibleApis);
            return await handler.Handle();
        }
    }
}
