using PortEval.Domain.Models.Entities;
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
    /// <summary>
    /// Aggregates multiple API clients supporting retrieval of instrument prices and currency exchange rates.
    /// </summary>
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

        /// <summary>
        /// Retrieves historical daily prices of the supplied symbol in the supplied range.
        /// </summary>
        /// <param name="instrument">Instrument to retrieve prices for.</param>
        /// <param name="from">Date range start</param>
        /// <param name="to">Date range end</param>
        /// <returns>A Response object containing operation status and historical prices if the operation was successful.</returns>
        public async Task<Response<IEnumerable<PricePoint>>> GetHistoricalDailyPrices(Instrument instrument, DateTime from,
            DateTime to)
        {
            var request = new HistoricalDailyInstrumentPricesRequest
            {
                Symbol = instrument.Symbol,
                Type = instrument.Type,
                CurrencyCode = instrument.CurrencyCode,
                From = TimeZoneInfo.ConvertTimeToUtc(from),
                To = TimeZoneInfo.ConvertTimeToUtc(to)
            };

            return await ProcessRequest<IHistoricalDailyFinancialApi, HistoricalDailyInstrumentPricesRequest,
                IEnumerable<PricePoint>>(request);
        }

        /// <summary>
        /// Retrieves intraday prices of the supplied symbol in the supplied range.
        /// </summary>
        /// <param name="instrument">Instrument to retrieve prices for.</param>
        /// <param name="from">Range start</param>
        /// <param name="to">Range end</param>
        /// <param name="interval">Interval between individual price points</param>
        /// <returns>A Response object containing operation status and intraday prices if the operation was successful.</returns>
        public async Task<Response<IEnumerable<PricePoint>>> GetIntradayPrices(Instrument instrument, DateTime from, DateTime to,
            IntradayInterval interval = IntradayInterval.OneHour)
        {
            var request = new IntradayPricesRequest
            {
                Symbol = instrument.Symbol,
                Type = instrument.Type,
                CurrencyCode = instrument.CurrencyCode,
                From = TimeZoneInfo.ConvertTimeToUtc(from),
                To = TimeZoneInfo.ConvertTimeToUtc(to),
                Interval = interval
            };

            return await ProcessRequest<IIntradayFinancialApi, IntradayPricesRequest, IEnumerable<PricePoint>>(request);
        }

        /// <summary>
        /// Retrieves the latest price of the supplied symbol.
        /// </summary>
        /// <param name="instrument">Instrument to retrieve prices for.</param>
        /// <returns>A Response object containing operation status and latest price point if the operation was successful.</returns>
        public async Task<Response<PricePoint>> GetLatestInstrumentPrice(Instrument instrument)
        {
            var request = new LatestInstrumentPriceRequest
            {
                Symbol = instrument.Symbol,
                Type = instrument.Type,
                CurrencyCode = instrument.CurrencyCode
            };

            return await ProcessRequest<ILatestPriceFinancialApi, LatestInstrumentPriceRequest, PricePoint>(request);
        }

        /// <summary>
        /// Retrieves the latest exchange rates of the supplied currency.
        /// </summary>
        /// <param name="baseCurrency">Base currency code</param>
        /// <returns>A Response object containing operation status and the latest exchange rates if the operation was successful.</returns>
        public async Task<Response<ExchangeRates>> GetLatestExchangeRates(string baseCurrency)
        {
            var request = new LatestExchangeRatesRequest
            {
                CurrencyCode = baseCurrency
            };

            return await ProcessRequest<ILatestExchangeRatesFinancialApi, LatestExchangeRatesRequest,
                ExchangeRates>(request);
        }

        /// <summary>
        /// Retrieves historical daily exchange rates of the supplied currency in the supplied range.
        /// </summary>
        /// <param name="baseCurrency">Base currency code</param>
        /// <param name="from">Date range start</param>
        /// <param name="to">Date range end</param>
        /// <returns>A Response object containing operation status and the retrieved exchange rates if the operation was successful.</returns>
        public async Task<Response<IEnumerable<ExchangeRates>>> GetHistoricalDailyExchangeRates(string baseCurrency, DateTime from, DateTime to)
        {
            var request = new HistoricalDailyExchangeRatesRequest
            {
                CurrencyCode = baseCurrency,
                From = TimeZoneInfo.ConvertTimeToUtc(from),
                To = TimeZoneInfo.ConvertTimeToUtc(to)
            };

            return await ProcessRequest<IHistoricalDailyExchangeRatesFinancialApi,
                HistoricalDailyExchangeRatesRequest, IEnumerable<ExchangeRates>>(request);
        }

        /// <summary>
        /// Processes the provided request and returns its response.
        /// </summary>
        /// <param name="request">Request to process</param>
        /// <typeparam name="TClient">Target API client type</typeparam>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResult">Response data type</typeparam>
        /// <returns></returns>
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
