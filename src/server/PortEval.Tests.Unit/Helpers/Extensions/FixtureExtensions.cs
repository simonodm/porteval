using AutoFixture;
using Moq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortEval.Tests.Unit.Helpers.Extensions
{
    internal static class FixtureExtensions
    {
        public static Mock<IFinancialDataFetcher> CreatePriceFetcherMockReturningHistoricalPrices(this IFixture fixture, Instrument instrument,
            IEnumerable<PricePoint> dailyPrices, IEnumerable<PricePoint> hourlyPrices,
            IEnumerable<PricePoint> fiveMinPrices)
        {
            var priceFetcher = fixture.Freeze<Mock<IFinancialDataFetcher>>();
            priceFetcher
                .Setup(m => m.GetHistoricalDailyPricesAsync(instrument.Symbol,
                    instrument.CurrencyCode, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(dailyPrices ?? Enumerable.Empty<PricePoint>());
            priceFetcher
                .Setup(m => m.GetIntradayPricesAsync(instrument.Symbol,
                    instrument.CurrencyCode, It.IsAny<DateTime>(), It.IsAny<DateTime>(), IntradayInterval.OneHour))
                .ReturnsAsync(hourlyPrices ?? Enumerable.Empty<PricePoint>());
            priceFetcher
                .Setup(m => m.GetIntradayPricesAsync(instrument.Symbol,
                    instrument.CurrencyCode, It.IsAny<DateTime>(), It.IsAny<DateTime>(), IntradayInterval.FiveMinutes))
                .ReturnsAsync(fiveMinPrices ?? Enumerable.Empty<PricePoint>());

            return priceFetcher;
        }

        /// <summary>
        /// Creates a <see cref="IPositionProfitCalculator" /> mock, the <see cref="IPositionProfitCalculator.CalculateProfit" /> of which
        /// is mocked to return the total amount of transactions which occurred before the time at which the profit is calculated,
        /// multiplied by the difference between the end price and start price of the position.
        /// </summary>
        /// <param name="fixture"></param>
        /// <returns></returns>
        public static Mock<IPositionProfitCalculator> CreatePositionProfitCalculatorMock(this IFixture fixture)
        {
            var profitCalculator = fixture.Freeze<Mock<IPositionProfitCalculator>>();

            // CalculateProfit is mocked to return the total amount of transactions which occurred before the time at which the profit is calculated,
            // multiplied by the difference between the end price and start price of the position
            profitCalculator
                .Setup(m => m.CalculateProfit(
                    It.IsAny<IEnumerable<PositionPriceRangeData>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .Returns<IEnumerable<PositionPriceRangeData>, DateTime, DateTime>(
                    (positionsPriceData, from, to) =>
                        from >= to ? 0m : positionsPriceData.Sum(p => p.Transactions
                            .Where(t => t.Time <= to)
                            .Sum(t => t.Amount * ((p.PriceAtRangeEnd?.Price ?? 0m) - (p.PriceAtRangeStart?.Price ?? 0m)))
                        )
                );

            return profitCalculator;
        }

        /// <summary>
        /// Creates a <see cref="IPositionPerformanceCalculator" /> mock, the <see cref="IPositionPerformanceCalculator.CalculatePerformance" /> of which
        /// is mocked to return the the total number of transactions which occurred before the time at which the performance is calculated
        /// </summary>
        /// <param name="fixture"></param>
        /// <returns></returns>
        public static Mock<IPositionPerformanceCalculator> CreatePositionPerformanceCalculatorMock(this IFixture fixture)
        {
            var performanceCalculator = fixture.Freeze<Mock<IPositionPerformanceCalculator>>();

            performanceCalculator
                .Setup(m => m.CalculatePerformance(
                    It.IsAny<IEnumerable<PositionPriceRangeData>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .Returns<IEnumerable<PositionPriceRangeData>, DateTime, DateTime>(
                    (positionsPriceData, from, to) =>
                        from >= to ? 0m : positionsPriceData.Sum(p => p.Transactions
                            .Where(t => t.Time <= to)
                            .Sum(t => t.Amount * ((p.PriceAtRangeEnd?.Price ?? 0m) - (p.PriceAtRangeStart?.Price ?? 0m)))
                        )
                );

            return performanceCalculator;
        }

        /// <summary>
        /// Creates a <see cref="IPositionBreakEvenPointCalculator" /> mock, the <see cref="IPositionBreakEvenPointCalculator.CalculatePositionBreakEvenPoint" /> of which
        /// is mocked to return the the total number of transactions which were supplied.
        /// </summary>
        /// <param name="fixture"></param>
        /// <returns></returns>
        public static Mock<IPositionBreakEvenPointCalculator> CreatePositionBEPCalculatorMock(this IFixture fixture)
        {
            var bepCalculator = fixture.Freeze<Mock<IPositionBreakEvenPointCalculator>>();
            bepCalculator
                .Setup(c => c.CalculatePositionBreakEvenPoint(It.IsAny<IEnumerable<TransactionDto>>()))
                .Returns<IEnumerable<TransactionDto>>(transactions => transactions.Count());

            return bepCalculator;
        }
    }
}