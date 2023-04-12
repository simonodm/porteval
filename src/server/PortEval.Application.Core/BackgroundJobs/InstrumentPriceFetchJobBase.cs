using PortEval.Application.Core.Interfaces;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Core.BackgroundJobs
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
                response = await PriceFetcher.GetHistoricalDailyCryptoPricesAsync(instrument.Symbol, instrument.CurrencyCode, from, to);
            }
            else
            {
                response = await PriceFetcher.GetHistoricalDailyPricesAsync(instrument.Symbol, instrument.CurrencyCode, from, to);
            }

            return response ?? Enumerable.Empty<PricePoint>();
        }

        protected virtual async Task<IEnumerable<PricePoint>> FetchIntradayPrices(Instrument instrument, DateTime from,
            DateTime to, IntradayInterval interval)
        {
            IEnumerable<PricePoint> response;

            if (instrument.Type == InstrumentType.CryptoCurrency)
            {
                response = await PriceFetcher.GetIntradayCryptoPricesAsync(instrument.Symbol, instrument.CurrencyCode, from, to, interval);
            }
            else
            {
                response = await PriceFetcher.GetIntradayPricesAsync(instrument.Symbol, instrument.CurrencyCode, from, to, interval);
            }

            return response ?? Enumerable.Empty<PricePoint>();
        }

        protected virtual async Task<PricePoint> FetchLatestPrice(Instrument instrument)
        {
            PricePoint response;

            if (instrument.Type == InstrumentType.CryptoCurrency)
            {
                response = await PriceFetcher.GetLatestCryptoPriceAsync(instrument.Symbol, instrument.CurrencyCode);
            }
            else
            {
                response = await PriceFetcher.GetLatestInstrumentPriceAsync(instrument.Symbol, instrument.CurrencyCode);
            }

            return response;
        }
    }
}
