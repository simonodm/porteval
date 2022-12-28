using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.FinancialDataFetcher.Interfaces;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.BackgroundJobs.Helpers;

namespace PortEval.BackgroundJobs
{
    public abstract class InstrumentPriceFetchJobBase
    {
        protected IPriceFetcher PriceFetcher { get; set; }

        protected InstrumentPriceFetchJobBase(IPriceFetcher priceFetcher)
        {
            PriceFetcher = priceFetcher;
        }

        protected virtual async Task<IEnumerable<PricePoint>> FetchHistoricalDailyPrices(Instrument instrument, DateTime from, DateTime to)
        {
            Response<IEnumerable<PricePoint>> response;

            if (instrument.Type == InstrumentType.CryptoCurrency)
            {
                response = await PriceFetcher.GetHistoricalDailyCryptoPrices(instrument.Symbol, instrument.CurrencyCode, from, to);
            }
            else
            {
                response = await PriceFetcher.GetHistoricalDailyPrices(instrument.Symbol, instrument.CurrencyCode, from, to);
            }

            return response.Result ?? Enumerable.Empty<PricePoint>();
        }

        protected virtual async Task<IEnumerable<PricePoint>> FetchIntradayPrices(Instrument instrument, DateTime from,
            DateTime to, IntradayInterval interval)
        {
            Response<IEnumerable<PricePoint>> response;

            if (instrument.Type == InstrumentType.CryptoCurrency)
            {
                response = await PriceFetcher.GetIntradayCryptoPrices(instrument.Symbol, instrument.CurrencyCode, from, to, interval);
            }
            else
            {
                response = await PriceFetcher.GetIntradayPrices(instrument.Symbol, instrument.CurrencyCode, from, to, interval);
            }

            return response.Result ?? Enumerable.Empty<PricePoint>();
        }

        protected virtual async Task<PricePoint> FetchLatestPrice(Instrument instrument)
        {
            Response<PricePoint> response;

            if (instrument.Type == InstrumentType.CryptoCurrency)
            {
                response = await PriceFetcher.GetLatestCryptoPrice(instrument.Symbol, instrument.CurrencyCode);
            }
            else
            {
                response = await PriceFetcher.GetLatestInstrumentPrice(instrument.Symbol, instrument.CurrencyCode);
            }

            return response.Result;
        }
    }
}
