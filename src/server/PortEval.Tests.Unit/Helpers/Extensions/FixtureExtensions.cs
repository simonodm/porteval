using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Tests.Extensions
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
                .Setup(m => m.Exists(It.IsAny<string>()))
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
                .Setup(m => m.Exists(It.IsAny<string>()))
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
                .Setup(m => m.Exists(It.IsAny<int>()))
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
                .Setup(m => m.Exists(It.IsAny<int>()))
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
                .Setup(m => m.ListAsync(It.IsAny<int>()))
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
                .Setup(m => m.FindParentPosition(It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.Create<Position>()));
            mock
                .Setup(m => m.Exists(It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            return mock;
        }

        public static Mock<IInstrumentPriceRepository> CreateDefaultInstrumentPriceRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            mock
                .Setup(m => m.FindPriceAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.Create<InstrumentPrice>()));
            mock
                .Setup(m => m.FindPriceAt(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(fixture.Create<InstrumentPrice>()));
            mock
                .Setup(m => m.Exists(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Exists(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.AddInstrumentPrice(It.IsAny<InstrumentPrice>()))
                .Returns<InstrumentPrice>(p => p);

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
                .Setup(m => m.Exists(It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            return mock;
        }

        public static Mock<IDashboardItemRepository> CreateDefaultDashboardItemRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IDashboardItemRepository>>();
            mock
                .Setup(m => m.GetDashboardItems())
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
                .Setup(m => m.Exists(It.IsAny<Guid>()))
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
    }
}