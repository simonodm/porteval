using AutoFixture;
using Moq;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.FinancialDataFetcher.Interfaces;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.Interfaces.Calculators;

namespace PortEval.Tests.Unit.Helpers.Extensions
{
    internal static class FixtureExtensions
    {
        #region Repository Mocks

        public static Mock<ICurrencyRepository> CreateDefaultCurrencyRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<ICurrencyRepository>>();
            mock
                .Setup(m => m.FindAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(fixture.Create<Currency>()));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Update(It.IsAny<Currency>()))
                .Returns<Currency>(c => c);
            mock
                .Setup(m => m.ListAllAsync())
                .Returns(Task.FromResult(fixture.CreateMany<Currency>()));
            mock
                .Setup(m => m.GetDefaultCurrencyAsync())
                .Returns(() =>
                {
                    var currency = fixture.Create<Currency>();
                    currency.SetAsDefault();
                    return Task.FromResult(currency);
                });

            return mock;
        }

        public static Mock<IExchangeRepository> CreateDefaultExchangeRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IExchangeRepository>>();
            mock
                .Setup(m => m.ListAllAsync())
                .Returns(Task.FromResult(fixture.CreateMany<Exchange>()));
            mock
                .Setup(m => m.FindAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(fixture.Create<Exchange>()));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Add(It.IsAny<Exchange>()))
                .Returns<Exchange>(e => e);

            return mock;
        }

        public static Mock<IInstrumentRepository> CreateDefaultInstrumentRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IInstrumentRepository>>();
            mock
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.Create<Instrument>()));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Add(It.IsAny<Instrument>()))
                .Returns<Instrument>(i => i);
            mock
                .Setup(m => m.Update(It.IsAny<Instrument>()))
                .Returns<Instrument>(i => i);
            mock
                .Setup(m => m.ListAllAsync())
                .Returns(Task.FromResult(fixture.CreateMany<Instrument>()));

            return mock;
        }

        public static Mock<IPortfolioRepository> CreateDefaultPortfolioRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IPortfolioRepository>>();
            mock
                .Setup(m => m.ListAllAsync())
                .Returns(Task.FromResult(fixture.CreateMany<Portfolio>()));
            mock
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.Create<Portfolio>()));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Add(It.IsAny<Portfolio>()))
                .Returns<Portfolio>(p => p);
            mock
                .Setup(m => m.Update(It.IsAny<Portfolio>()))
                .Returns<Portfolio>(p => p);

            return mock;
        }

        public static Mock<IPositionRepository> CreateDefaultPositionRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IPositionRepository>>();
            mock
                .Setup(m => m.ListPortfolioPositionsAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.CreateMany<Position>()));
            mock
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.Create<Position>()));
            mock
                .Setup(m => m.Add(It.IsAny<Position>()))
                .Returns<Position>(p => p);
            mock
                .Setup(m => m.Update(It.IsAny<Position>()))
                .Returns<Position>(p => p);
            mock
                .Setup(m => m.FindParentPositionAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.Create<Position>()));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            return mock;
        }

        public static Mock<IInstrumentPriceRepository> CreateDefaultInstrumentPriceRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            mock
                .Setup(m => m.FindPriceByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.Create<InstrumentPrice>()));
            mock
                .Setup(m => m.FindPriceAtAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(fixture.Create<InstrumentPrice>()));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Add(It.IsAny<InstrumentPrice>()))
                .Returns<InstrumentPrice>(p => p);

            return mock;
        }

        public static Mock<ICurrencyExchangeRateRepository> CreateDefaultCurrencyExchangeRateRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<ICurrencyExchangeRateRepository>>();
            mock
                .Setup(m => m.ListExchangeRatesAsync(It.IsAny<string>()))
                .ReturnsAsync(fixture.CreateMany<CurrencyExchangeRate>());
            mock
                .Setup(m => m.GetExchangeRateAtAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns<string, string, DateTime>((from, to, time) => Task.FromResult(new CurrencyExchangeRate(time, 1, from, to)));
            mock
                .Setup(m => m.Add(It.IsAny<CurrencyExchangeRate>()))
                .Returns<CurrencyExchangeRate>(er => er);

            return mock;
        }
        public static Mock<IChartRepository> CreateDefaultChartRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IChartRepository>>();
            mock
                .Setup(m => m.ListAllAsync())
                .Returns(Task.FromResult(fixture.CreateMany<Chart>()));
            mock
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.Create<Chart>()));
            mock
                .Setup(m => m.Add(It.IsAny<Chart>()))
                .Returns<Chart>(c => c);
            mock
                .Setup(m => m.Update(It.IsAny<Chart>()))
                .Returns<Chart>(c => c);
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            return mock;
        }

        public static Mock<IDashboardItemRepository> CreateDefaultDashboardItemRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IDashboardItemRepository>>();
            mock
                .Setup(m => m.GetDashboardItemsAsync())
                .ReturnsAsync(fixture.CreateMany<DashboardChartItem>());
            mock
                .Setup(m => m.Add(It.IsAny<DashboardItem>()))
                .Returns<DashboardItem>(i => i);
            mock
                .Setup(m => m.Update(It.IsAny<DashboardItem>()))
                .Returns<DashboardItem>(i => i);

            return mock;
        }

        public static Mock<IDataImportRepository> CreateDefaultDataImportRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IDataImportRepository>>();
            mock
                .Setup(m => m.ListAllAsync())
                .Returns(Task.FromResult(fixture.CreateMany<DataImport>()));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Add(It.IsAny<DataImport>()))
                .Returns<DataImport>(i => i);
            mock
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(i => i);

            return mock;
        }

        #endregion

        #region Service Mocks

        public static Mock<IInstrumentPriceService> CreateDefaultInstrumentPriceServiceMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IInstrumentPriceService>>();
            mock
                .Setup(m => m.AddPricePointAsync(It.IsAny<InstrumentPriceDto>()))
                .Returns(Task.FromResult(fixture.Create<InstrumentPrice>()));
            mock
                .Setup(m => m.AddPriceIfNotExistsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<decimal>()))
                .Returns(Task.FromResult(fixture.Create<InstrumentPrice>()));

            return mock;
        }

        public static Mock<IPortfolioService> CreateDefaultPortfolioServiceMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IPortfolioService>>();
            mock
                .Setup(m => m.CreatePortfolioAsync(It.IsAny<PortfolioDto>()))
                .ReturnsAsync(fixture.Create<Portfolio>());
            mock
                .Setup(m => m.UpdatePortfolioAsync(It.IsAny<PortfolioDto>()))
                .ReturnsAsync(fixture.Create<Portfolio>());

            return mock;
        }

        public static Mock<IPositionService> CreateDefaultPositionServiceMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IPositionService>>();
            mock
                .Setup(m => m.OpenPositionAsync(It.IsAny<PositionDto>()))
                .ReturnsAsync(fixture.Create<Position>());
            mock
                .Setup(m => m.UpdatePositionAsync(It.IsAny<PositionDto>()))
                .ReturnsAsync(fixture.Create<Position>());

            return mock;
        }

        public static Mock<IInstrumentService> CreateDefaultInstrumentServiceMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IInstrumentService>>();
            mock
                .Setup(m => m.CreateInstrumentAsync(It.IsAny<InstrumentDto>()))
                .ReturnsAsync(fixture.Create<Instrument>());
            mock
                .Setup(m => m.UpdateInstrumentAsync(It.IsAny<InstrumentDto>()))
                .ReturnsAsync(fixture.Create<Instrument>());

            return mock;
        }

        public static Mock<ITransactionService> CreateDefaultTransactionServiceMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<ITransactionService>>();
            mock
                .Setup(m => m.AddTransactionAsync(It.IsAny<TransactionDto>()))
                .ReturnsAsync(fixture.Create<Transaction>());
            mock
                .Setup(m => m.UpdateTransactionAsync(It.IsAny<TransactionDto>()))
                .ReturnsAsync(fixture.Create<Transaction>());

            return mock;
        }

        #endregion

        public static Mock<IPriceFetcher> CreatePriceFetcherMockReturningHistoricalPrices(this IFixture fixture, Instrument instrument,
            IEnumerable<PricePoint> dailyPrices, IEnumerable<PricePoint> hourlyPrices,
            IEnumerable<PricePoint> fiveMinPrices)
        {
            var priceFetcher = fixture.Freeze<Mock<IPriceFetcher>>();
            priceFetcher
                .Setup(m => m.GetHistoricalDailyPrices(instrument, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new Response<IEnumerable<PricePoint>>
                {
                    StatusCode = StatusCode.Ok,
                    Result = dailyPrices ?? Enumerable.Empty<PricePoint>()
                });
            priceFetcher
                .Setup(m => m.GetIntradayPrices(instrument, It.IsAny<DateTime>(), It.IsAny<DateTime>(), IntradayInterval.OneHour))
                .ReturnsAsync(new Response<IEnumerable<PricePoint>>
                {
                    StatusCode = StatusCode.Ok,
                    Result = hourlyPrices ?? Enumerable.Empty<PricePoint>()
                });
            priceFetcher
                .Setup(m => m.GetIntradayPrices(instrument, It.IsAny<DateTime>(), It.IsAny<DateTime>(), IntradayInterval.FiveMinutes))
                .ReturnsAsync(new Response<IEnumerable<PricePoint>>
                {
                    StatusCode = StatusCode.Ok,
                    Result = fiveMinPrices ?? Enumerable.Empty<PricePoint>()
                });

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