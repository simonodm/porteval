using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.BackgroundJobs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests
{
    public class SplitPriceAndTransactionAdjustmentJobTests
    {
        [Fact]
        public async Task Run_DividesPricesBeforeSplitBySplitFactor_WhenSplitIsNonProcessed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var firstPrice = 100m;
            var secondPrice = 200m;

            var prices = new[]
            {
                new InstrumentPrice(1, DateTime.Parse("2022-01-01"), DateTime.Parse("2022-01-01"), firstPrice, 1),
                new InstrumentPrice(2, DateTime.Parse("2022-02-01"), DateTime.Parse("2022-02-01"), secondPrice, 1),
            };

            var splits = new[]
            {
                new InstrumentSplit(1, 1, DateTime.UtcNow, new SplitRatio(1, 5))
            };

            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();
            splitRepository
                .Setup(r => r.ListNonProcessedSplitsAsync())
                .ReturnsAsync(splits);
            splitRepository
                .Setup(r => r.ListRollbackRequestedSplitsAsync())
                .ReturnsAsync(Enumerable.Empty<InstrumentSplit>());
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            priceRepository
                .Setup(m => m.ListInstrumentPricesAsync(It.IsAny<int>()))
                .ReturnsAsync(prices);

            var sut = fixture.Create<SplitPriceAndTransactionAdjustmentJob>();

            await sut.Run();

            priceRepository
                .Verify(r => r.Update(It.Is<InstrumentPrice>(price =>
                    price.Id == prices[0].Id && price.Price == firstPrice / 5
            )));
            priceRepository
                .Verify(r => r.Update(It.Is<InstrumentPrice>(price =>
                    price.Id == prices[1].Id && price.Price == secondPrice / 5
            )));
        }

        [Fact]
        public async Task Run_MultipliesTransactionsAmountBySplitFactor_WhenSplitIsNonProcessed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var firstAmount = 100m;
            var secondAmount = 200m;

            var position = new Position(1, 1, 1, "");
            position.AddTransaction(firstAmount, fixture.Create<decimal>(), DateTime.Parse("2022-01-01"));
            position.AddTransaction(secondAmount, fixture.Create<decimal>(), DateTime.Parse("2022-02-01"));

            var splits = new[]
            {
                new InstrumentSplit(1, 1, DateTime.UtcNow, new SplitRatio(1, 5))
            };

            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();
            splitRepository
                .Setup(r => r.ListNonProcessedSplitsAsync())
                .ReturnsAsync(splits);
            splitRepository
                .Setup(r => r.ListRollbackRequestedSplitsAsync())
                .ReturnsAsync(Enumerable.Empty<InstrumentSplit>());
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.ListAllInstrumentPositionsAsync(1))
                .ReturnsAsync(new[] { position });

            var sut = fixture.Create<SplitPriceAndTransactionAdjustmentJob>();

            await sut.Run();

            Assert.Equal(position.Transactions.First().Amount, firstAmount * 5);
            Assert.Equal(position.Transactions.Last().Amount, secondAmount * 5);
            positionRepository.Verify(r => r.Update(position));
        }

        [Fact]
        public async Task Run_DividesTransactionsPriceBySplitFactor_WhenSplitIsNonProcessed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var firstPrice = 100m;
            var secondPrice = 200m;

            var position = new Position(1, 1, 1, "");
            position.AddTransaction(1, firstPrice, DateTime.Parse("2022-01-01"));
            position.AddTransaction(1, secondPrice, DateTime.Parse("2022-02-01"));

            var splits = new[]
            {
                new InstrumentSplit(1, 1, DateTime.UtcNow, new SplitRatio(1, 5))
            };

            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();
            splitRepository
                .Setup(r => r.ListNonProcessedSplitsAsync())
                .ReturnsAsync(splits);
            splitRepository
                .Setup(r => r.ListRollbackRequestedSplitsAsync())
                .ReturnsAsync(Enumerable.Empty<InstrumentSplit>());
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.ListAllInstrumentPositionsAsync(1))
                .ReturnsAsync(new[] { position });

            var sut = fixture.Create<SplitPriceAndTransactionAdjustmentJob>();

            await sut.Run();

            Assert.Equal(position.Transactions.First().Price, firstPrice / 5);
            Assert.Equal(position.Transactions.Last().Price, secondPrice / 5);
            positionRepository.Verify(r => r.Update(position));
        }

        [Fact]
        public async Task Run_MultipliesPricesBeforeSplitBySplitFactor_WhenSplitRollbackIsRequested()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var firstPrice = 100m;
            var secondPrice = 200m;

            var prices = new[]
            {
                new InstrumentPrice(1, DateTime.Parse("2022-01-01"), DateTime.Parse("2022-01-01"), firstPrice, 1),
                new InstrumentPrice(2, DateTime.Parse("2022-02-01"), DateTime.Parse("2022-02-01"), secondPrice, 1),
            };

            var split = new InstrumentSplit(1, 1, DateTime.UtcNow, new SplitRatio(1, 5));
            split.Rollback();

            var splits = new[]
            {
                split
            };

            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();
            splitRepository
                .Setup(r => r.ListRollbackRequestedSplitsAsync())
                .ReturnsAsync(splits);
            splitRepository
                .Setup(r => r.ListNonProcessedSplitsAsync())
                .ReturnsAsync(Enumerable.Empty<InstrumentSplit>());
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            priceRepository
                .Setup(m => m.ListInstrumentPricesAsync(It.IsAny<int>()))
                .ReturnsAsync(prices);

            var sut = fixture.Create<SplitPriceAndTransactionAdjustmentJob>();

            await sut.Run();

            priceRepository
                .Verify(r => r.Update(It.Is<InstrumentPrice>(price =>
                    price.Id == prices[0].Id && price.Price == firstPrice * 5
            )));
            priceRepository
                .Verify(r => r.Update(It.Is<InstrumentPrice>(price =>
                    price.Id == prices[1].Id && price.Price == secondPrice * 5
            )));
        }

        [Fact]
        public async Task Run_DividesTransactionsAmountBySplitFactor_WhenSplitRollbackIsRequested()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var firstAmount = 100m;
            var secondAmount = 200m;

            var position = new Position(1, 1, 1, "");
            position.AddTransaction(firstAmount, fixture.Create<decimal>(), DateTime.Parse("2022-01-01"));
            position.AddTransaction(secondAmount, fixture.Create<decimal>(), DateTime.Parse("2022-02-01"));

            var split = new InstrumentSplit(1, 1, DateTime.UtcNow, new SplitRatio(1, 5));
            split.Rollback();

            var splits = new[]
            {
                split
            };

            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();
            splitRepository
                .Setup(r => r.ListRollbackRequestedSplitsAsync())
                .ReturnsAsync(splits);
            splitRepository
                .Setup(r => r.ListNonProcessedSplitsAsync())
                .ReturnsAsync(Enumerable.Empty<InstrumentSplit>());
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.ListAllInstrumentPositionsAsync(1))
                .ReturnsAsync(new[] { position });

            var sut = fixture.Create<SplitPriceAndTransactionAdjustmentJob>();

            await sut.Run();

            Assert.Equal(position.Transactions.First().Amount, firstAmount / 5);
            Assert.Equal(position.Transactions.Last().Amount, secondAmount / 5);
            positionRepository.Verify(r => r.Update(position));
        }

        [Fact]
        public async Task Run_MultipliesTransactionsPriceBySplitFactor_WhenSplitRollbackIsRequested()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var firstPrice = 100m;
            var secondPrice = 200m;

            var position = new Position(1, 1, 1, "");
            position.AddTransaction(1, firstPrice, DateTime.Parse("2022-01-01"));
            position.AddTransaction(1, secondPrice, DateTime.Parse("2022-02-01"));

            var split = new InstrumentSplit(1, 1, DateTime.UtcNow, new SplitRatio(1, 5));
            split.Rollback();

            var splits = new[]
            {
                split
            };

            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();
            splitRepository
                .Setup(r => r.ListRollbackRequestedSplitsAsync())
                .ReturnsAsync(splits);
            splitRepository
                .Setup(r => r.ListNonProcessedSplitsAsync())
                .ReturnsAsync(Enumerable.Empty<InstrumentSplit>());
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.ListAllInstrumentPositionsAsync(1))
                .ReturnsAsync(new[] { position });

            var sut = fixture.Create<SplitPriceAndTransactionAdjustmentJob>();

            await sut.Run();

            Assert.Equal(position.Transactions.First().Price, firstPrice * 5);
            Assert.Equal(position.Transactions.Last().Price, secondPrice * 5);
            positionRepository.Verify(r => r.Update(position));
        }
    }
}
