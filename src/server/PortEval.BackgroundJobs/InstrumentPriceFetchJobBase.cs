using PortEval.Application.Features.Interfaces;
using PortEval.Application.Models.PriceFetcher;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.BackgroundJobs
{
    public abstract class InstrumentPriceFetchJobBase
    {
        protected IFinancialDataFetcher PriceFetcher { get; set; }

        protected InstrumentPriceFetchJobBase(IFinancialDataFetcher priceFetcher)
        {
            PriceFetcher = priceFetcher;
        }

        protected virtual async Task<IEnumerable<PricePoint>> FetchHistoricalDailyPrices(Instrument instrument, DateTime from, DateTime to)
        {
            IEnumerable<PricePoint> response;

            if (instrument.Type == InstrumentType.CryptoCurrency)
            {
                response = await PriceFetcher.GetHistoricalDailyCryptoPrices(instrument.Symbol, instrument.CurrencyCode, from, to);
            }
            else
            {
                response = await PriceFetcher.GetHistoricalDailyPrices(instrument.Symbol, instrument.CurrencyCode, from, to);
            }

            return response ?? Enumerable.Empty<PricePoint>();
        }

        protected virtual async Task<IEnumerable<PricePoint>> FetchIntradayPrices(Instrument instrument, DateTime from,
            DateTime to, IntradayInterval interval)
        {
            IEnumerable<PricePoint> response;

            if (instrument.Type == InstrumentType.CryptoCurrency)
            {
                response = await PriceFetcher.GetIntradayCryptoPrices(instrument.Symbol, instrument.CurrencyCode, from, to, interval);
            }
            else
            {
                response = await PriceFetcher.GetIntradayPrices(instrument.Symbol, instrument.CurrencyCode, from, to, interval);
            }

            return response ?? Enumerable.Empty<PricePoint>();
        }

        protected virtual async Task<PricePoint> FetchLatestPrice(Instrument instrument)
        {
            PricePoint response;

            if (instrument.Type == InstrumentType.CryptoCurrency)
            {
                response = await PriceFetcher.GetLatestCryptoPrice(instrument.Symbol, instrument.CurrencyCode);
            }
            else
            {
                response = await PriceFetcher.GetLatestInstrumentPrice(instrument.Symbol, instrument.CurrencyCode);
            }

            return response;
        }
    }
}
