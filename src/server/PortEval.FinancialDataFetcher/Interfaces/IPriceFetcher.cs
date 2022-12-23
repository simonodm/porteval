using PortEval.Domain.Models.Entities;
using PortEval.FinancialDataFetcher.Interfaces.APIs;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher.Interfaces
{
    public interface IPriceFetcher
    {
        public Task<Response<IEnumerable<PricePoint>>> GetHistoricalDailyPrices(Instrument instrument, DateTime from, DateTime to);
        public Task<Response<IEnumerable<PricePoint>>> GetIntradayPrices(Instrument instrument, DateTime from, DateTime to,
            IntradayInterval interval = IntradayInterval.OneHour);
        public Task<Response<PricePoint>> GetLatestInstrumentPrice(Instrument instrument);
        public Task<Response<ExchangeRates>> GetLatestExchangeRates(string baseCurrency);
        public Task<Response<IEnumerable<ExchangeRates>>> GetHistoricalDailyExchangeRates(string baseCurrency, DateTime from, DateTime to);
        public Task<Response<TResult>> ProcessRequest<TClient, TRequest, TResult>(TRequest request)
            where TClient : class, IFinancialApi<TRequest, Response<TResult>>
            where TRequest : IRequest;
    }
}
