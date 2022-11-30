using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.FinancialDataFetcher.Models;
using PortEval.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.BackgroundJobs
{
    internal static class PriceUtils
    {
        public static TimeSpan FiveMinutes => TimeSpan.FromMinutes(5);
        public static TimeSpan OneHour => TimeSpan.FromHours(1);
        public static TimeSpan OneDay => TimeSpan.FromDays(1);
        public static TimeSpan FiveDays => TimeSpan.FromDays(5);

        /// <summary>
        /// Calculates missing instrument price ranges according to the intervals supplied in the <c>intervalFunction</c> callback.
        /// </summary>
        /// <param name="existingPriceTimes">All existing instrument prices.</param>
        /// <param name="intervalFunction">Callback to use for desired price interval calculation.</param>
        /// <param name="rangeStart">Date and time to detect missing ranges from.</param>
        /// <param name="rangeEnd">Date and time to detect missing ranges until.</param>
        /// <returns>An <c>IEnumerable</c> containing all missing time ranges.</returns>
        public static IEnumerable<TimeRange> GetMissingPriceRanges(IEnumerable<DateTime> existingPriceTimes,
            Func<DateTime, DateTime, TimeSpan> intervalFunction, DateTime rangeStart, DateTime rangeEnd)
        {
            var result = new List<TimeRange>();
            DateTime? previousPriceTime = null;

            foreach (var priceTime in existingPriceTimes.Append(rangeStart).Append(rangeEnd).OrderBy(t => t))
            {
                previousPriceTime ??= priceTime;
                var interval = intervalFunction(rangeEnd, priceTime);
                if (priceTime - previousPriceTime > interval)
                {
                    result.Add(new TimeRange
                    {
                        From = (DateTime)previousPriceTime,
                        To = priceTime,
                        Interval = interval
                    });
                }

                previousPriceTime = priceTime;
            }

            return result;
        }

        /// <summary>
        /// Fills price ranges of retrieved prices to maintain the desired intervals even in the absence of exchange price data (e. g. during times when
        /// the exchange is closed). For each missing time T it creates a new price point with the last available price before T.
        /// </summary>
        /// <param name="fetchedPrices">Retrieved prices.</param>
        /// <param name="rangeStart">Fetch range start.</param>
        /// <param name="rangeEnd">Fetch range end.</param>
        /// <returns>An <c>IEnumerable</c> of supplied prices together with the filled data.</returns>
        public static IEnumerable<PricePoint> FillMissingRangePrices(IEnumerable<PricePoint> fetchedPrices, DateTime rangeStart, DateTime rangeEnd)
        {
            var pricePoints = fetchedPrices.ToList();
            var missingRanges =
                GetMissingPriceRanges(pricePoints.Select(p => p.Time), GetInstrumentPriceInterval, rangeStart, rangeEnd);

            var result = new List<PricePoint>();
            foreach (var range in missingRanges)
            {
                var latestPrice = pricePoints.OrderByDescending(p => p.Time)
                    .FirstOrDefault(p => p.Time <= range.From);

                var currentTime = range.From;
                while (currentTime < range.To)
                {
                    if (currentTime > range.From)
                    {
                        result.Add(new PricePoint
                        {
                            CurrencyCode = latestPrice!.CurrencyCode,
                            Price = latestPrice.Price,
                            Symbol = latestPrice.Symbol,
                            Time = currentTime
                        });
                    }

                    currentTime += range.Interval;
                }
            }

            pricePoints.AddRange(result);
            return pricePoints;
        }

        /// <summary>
        /// Retrieves expected price time interval to maintain in the target time.
        /// </summary>
        /// <param name="baseTime">Base time to use for interval requirements.</param>
        /// <param name="targetTime">Target time.</param>
        /// <returns>A <c>TimeSpan</c> representing the interval to maintain.</returns>
        public static TimeSpan GetInstrumentPriceInterval(DateTime baseTime, DateTime targetTime)
        {
            if (baseTime - targetTime <= TimeSpan.FromDays(1))
            {
                return FiveMinutes;
            }

            if (baseTime - targetTime <= TimeSpan.FromDays(5))
            {
                return OneHour;
            }

            return OneDay;
        }

        /// <summary>
        /// Retrieves expected exchange rate interval to maintain in the target time.
        /// </summary>
        /// <param name="baseTime">Base time to use for interval requirements.</param>
        /// <param name="targetTime">Target time.</param>
        /// <returns>A <c>TimeSpan</c> representing the interval to maintain.</returns>
        public static TimeSpan GetCurrencyExchangeRateInterval(DateTime baseTime, DateTime targetTime)
        {
            return OneDay;
        }

        /// <summary>
        /// Converts the given price point to the instrument's currency.
        /// </summary>
        /// <param name="context">PortEval EF context.</param>
        /// <param name="instrument">Instrument to use currency of.</param>
        /// <param name="pricePoint">Price point to convert.</param>
        /// <returns>A task representing the asynchronous conversion operation. The task result contains the converted price.</returns>
        public static async Task<decimal> GetConvertedPricePointPrice(ICurrencyExchangeRateRepository exchangeRateRepository, Instrument instrument, PricePoint pricePoint)
        {
            var price = pricePoint.Price;
            if (pricePoint.CurrencyCode == instrument.CurrencyCode) return price;

            var conversionRate = await exchangeRateRepository.GetExchangeRateAtAsync(pricePoint.CurrencyCode, instrument.CurrencyCode, pricePoint.Time);

            if(conversionRate == null)
            {
                throw new ApplicationException($"No conversion rate from {pricePoint.CurrencyCode} to {instrument.CurrencyCode} found at {pricePoint.Time}");
            }

            var convertedPrice = pricePoint.Price * conversionRate.ExchangeRate;
            return convertedPrice;
        }
    }
}
