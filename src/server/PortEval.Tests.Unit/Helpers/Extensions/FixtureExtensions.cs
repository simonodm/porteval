using AutoFixture;
using Moq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Tests.Unit.Helpers.Extensions
{
    internal static class FixtureExtensions
    {
        #region Query Mocks

        public static Mock<IChartQueries> CreateDefaultChartQueriesMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IChartQueries>>();
            mock
                .Setup(m => m.GetChartsAsync())
                .ReturnsAsync(fixture.CreateMany<ChartDto>());
            mock
                .Setup(m => m.GetChartAsync(It.IsAny<int>()))
                .ReturnsAsync(fixture.Create<ChartDto>());

            return mock;
        }

        public static Mock<ICurrencyQueries> CreateDefaultCurrencyQueriesMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<ICurrencyQueries>>();
            mock
                .Setup(m => m.GetAllCurrenciesAsync())
                .ReturnsAsync(fixture.CreateMany<CurrencyDto>());
            mock
                .Setup(m => m.GetCurrencyAsync(It.IsAny<string>()))
                .ReturnsAsync(fixture.Create<CurrencyDto>());
            mock
                .Setup(m => m.GetDefaultCurrencyAsync())
                .ReturnsAsync(fixture.Create<CurrencyDto>());
            mock
                .Setup(
                    m => m.GetCurrencyExchangeRateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(fixture.Create<CurrencyExchangeRateDto>());
            mock
                .Setup(m => m.GetDirectExchangeRatesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(fixture.CreateMany<CurrencyExchangeRateDto>());
            mock
                .Setup(m => m.GetDirectExchangeRatesAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<DateRangeParams>()))
                .ReturnsAsync(fixture.CreateMany<CurrencyExchangeRateDto>());
            mock
                .Setup(m => m.GetInversedExchangeRatesAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<DateRangeParams>()))
                .ReturnsAsync(fixture.CreateMany<CurrencyExchangeRateDto>());

            return mock;
        }

        public static Mock<IDashboardLayoutQueries> CreateDefaultDashboardLayoutQueriesMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IDashboardLayoutQueries>>();
            mock
                .Setup(m => m.GetDashboardItemsAsync())
                .ReturnsAsync(fixture.CreateMany<DashboardItemDto>());

            return mock;
        }

        public static Mock<IDataImportQueries> CreateDefaultDataImportQueriesMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IDataImportQueries>>();
            mock
                .Setup(m => m.GetAllImportsAsync())
                .ReturnsAsync(fixture.CreateMany<CsvTemplateImportDto>());
            mock
                .Setup(m => m.GetImportAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fixture.Create<CsvTemplateImportDto>());

            return mock;
        }

        public static Mock<IExchangeQueries> CreateDefaultExchangeQueriesMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IExchangeQueries>>();
            mock
                .Setup(m => m.GetKnownExchangesAsync())
                .ReturnsAsync(fixture.CreateMany<ExchangeDto>());

            return mock;
        }

        public static Mock<IInstrumentQueries> CreateDefaultInstrumentQueriesMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IInstrumentQueries>>();
            mock
                .Setup(m => m.GetAllInstrumentsAsync())
                .ReturnsAsync(fixture.CreateMany<InstrumentDto>());
            mock
                .Setup(m => m.GetInstrumentPageAsync(It.IsAny<PaginationParams>()))
                .ReturnsAsync(fixture.CreateMany<InstrumentDto>());
            mock
                .Setup(m => m.GetInstrumentCountAsync())
                .ReturnsAsync(fixture.Create<int>());
            mock
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync(fixture.Create<InstrumentDto>());
            mock
                .Setup(m => m.GetInstrumentPricesAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(fixture.CreateMany<InstrumentPriceDto>());
            mock
                .Setup(m => m.GetInstrumentPricesPageAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<PaginationParams>(), It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(fixture.CreateMany<InstrumentPriceDto>());
            mock
                .Setup(m => m.GetInstrumentPricesPageCompressedAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<PaginationParams>(), It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(fixture.CreateMany<InstrumentPriceDto>());
            mock
                .Setup(m => m.GetInstrumentPriceCountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(fixture.Create<int>());
            mock
                .Setup(m => m.GetInstrumentPriceCompressedCountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<AggregationFrequency?>()))
                .ReturnsAsync(fixture.Create<int>());

            return mock;
        }

        public static Mock<IPortfolioQueries> CreateDefaultPortfolioQueriesMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IPortfolioQueries>>();
            mock
                .Setup(m => m.GetAllPortfoliosAsync())
                .ReturnsAsync(fixture.CreateMany<PortfolioDto>());
            mock
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync(fixture.Create<PortfolioDto>());

            return mock;
        }

        public static Mock<IPositionQueries> CreateDefaultPositionQueriesMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IPositionQueries>>();
            mock
                .Setup(m => m.GetAllPositionsAsync())
                .ReturnsAsync(fixture.CreateMany<PositionDto>());
            mock
                .Setup(m => m.GetPortfolioPositionsAsync(It.IsAny<int>()))
                .ReturnsAsync(fixture.CreateMany<PositionDto>());
            mock
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync(fixture.Create<PositionDto>());

            return mock;
        }

        public static Mock<ITransactionQueries> CreateDefaultTransactionQueriesMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<ITransactionQueries>>();
            mock
                .Setup(m => m.GetTransactionsAsync(It.IsAny<TransactionFilters>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(fixture.CreateMany<TransactionDto>());
            mock
                .Setup(m => m.GetTransactionAsync(It.IsAny<int>()))
                .ReturnsAsync(fixture.Create<TransactionDto>());

            return mock;
        }

        #endregion

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
                    var currency = new Currency(fixture.Create<string>(), fixture.Create<string>(),
                        fixture.Create<string>(), true);
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
                .Returns<int>(id => Task.FromResult(
                    new Instrument(
                        id,
                        fixture.Create<string>(),
                        fixture.Create<string>(),
                        fixture.Create<string>(),
                        InstrumentType.Stock,
                        fixture.Create<string>(),
                        fixture.Create<string>()
                    ))
                );
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
                .Returns<int>(id => Task.FromResult(new Portfolio(id, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>())));
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
                .Returns(Task.FromResult((IEnumerable<Position>)new List<Position> { new Position(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<int>(), fixture.Create<string>()) }));
            mock
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(new Position(id, fixture.Create<int>(), fixture.Create<int>(), fixture.Create<string>())));
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
                .Returns<int, int>((instrumentId, priceId) => Task.FromResult(
                    new InstrumentPrice(priceId, fixture.Create<DateTime>(), fixture.Create<DateTime>(), fixture.Create<decimal>(), instrumentId)
                ));
            mock
                .Setup(m => m.FindPriceAtAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns<int, DateTime>((instrumentId, dt) => Task.FromResult(
                    new InstrumentPrice(fixture.Create<int>(), dt, dt, fixture.Create<decimal>(), instrumentId)
                ));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Add(It.IsAny<InstrumentPrice>()))
                .Returns<InstrumentPrice>(p => p);
            mock
                .Setup(m => m.Update(It.IsAny<InstrumentPrice>()))
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
                .Returns<int>(id => Task.FromResult(new Chart(id, fixture.Create<string>(), fixture.Create<ChartDateRange>(), fixture.Create<ChartTypeSettings>())));
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

        public static Mock<IInstrumentSplitRepository> CreateDefaultSplitRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IInstrumentSplitRepository>>();
            mock
                .Setup(m => m.ListInstrumentSplitsAsync(It.IsAny<int>()))
                .ReturnsAsync(fixture.CreateMany<InstrumentSplit>());
            mock
                .Setup(m => m.ListNonProcessedSplitsAsync())
                .ReturnsAsync(fixture.CreateMany<InstrumentSplit>());
            mock
                .Setup(m => m.ListRollbackRequestedSplitsAsync())
                .ReturnsAsync(fixture.CreateMany<InstrumentSplit>());
            mock
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(new InstrumentSplit(id, fixture.Create<int>(),
                    fixture.Create<DateTime>(), fixture.Create<SplitRatio>())));
            mock
                .Setup(m => m.Add(It.IsAny<InstrumentSplit>()))
                .Returns<InstrumentSplit>(s => s);
            mock
                .Setup(m => m.Update(It.IsAny<InstrumentSplit>()))
                .Returns<InstrumentSplit>(s => s);
            mock
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            return mock;
        }

        #endregion

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