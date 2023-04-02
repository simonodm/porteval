using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests
{
    public class InstrumentPriceCleanupJobTests
    {
        [Fact]
        public async Task Run_MaintainsOneDayIntervalForPricesOlderThan5Days()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var baseTime = DateTime.UtcNow;

            var instruments = new List<Instrument>
            {
                new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "")
            };

            var prices = new List<InstrumentPrice>
            {
                new InstrumentPrice(1, baseTime.AddDays(-5), baseTime, 100, 1),
                new InstrumentPrice(2, baseTime.AddDays(-5).AddHours(-12), baseTime, 100, 1),
                new InstrumentPrice(3, baseTime.AddDays(-6), baseTime, 100, 1),
                new InstrumentPrice(4, baseTime.AddDays(-6).AddHours(-12), baseTime, 100, 1),
                new InstrumentPrice(5, baseTime.AddDays(-7), baseTime, 100, 1)
            };

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(instruments);

            var priceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            priceRepository
                .Setup(m => m.ListInstrumentPricesAsync(instruments[0].Id))
                .ReturnsAsync(prices);

            var sut = fixture.Create<InstrumentPriceCleanupJob>();

            await sut.Run();

            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 1)), Times.Never());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 2)), Times.Once());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 3)), Times.Never());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 4)), Times.Once());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 5)), Times.Never());
        }

        [Fact]
        public async Task Run_MaintainsOneHourInterval_ForPricesInThePastFiveDays()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var baseTime = DateTime.UtcNow;

            var instruments = new List<Instrument>
            {
                new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "")
            };

            var prices = new List<InstrumentPrice>
            {
                new InstrumentPrice(1, baseTime.AddDays(-1), baseTime, 100, 1),
                new InstrumentPrice(2, baseTime.AddDays(-1).AddMinutes(-30), baseTime, 100, 1),
                new InstrumentPrice(3, baseTime.AddDays(-1).AddHours(-1), baseTime, 100, 1),
                new InstrumentPrice(4, baseTime.AddDays(-4).AddHours(-23), baseTime, 100, 1),
                new InstrumentPrice(5, baseTime.AddDays(-4).AddHours(-23).AddMinutes(-30), baseTime, 100, 1),
                new InstrumentPrice(6, baseTime.AddDays(-5), baseTime, 100, 1)
            };

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(instruments);

            var priceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            priceRepository
                .Setup(m => m.ListInstrumentPricesAsync(instruments[0].Id))
                .ReturnsAsync(prices);

            var sut = fixture.Create<InstrumentPriceCleanupJob>();

            await sut.Run();

            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 1)), Times.Never());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 2)), Times.Once());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 3)), Times.Never());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 4)), Times.Never());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 5)), Times.Once());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 6)), Times.Never());
        }

        [Fact]
        public async Task Run_MaintainsFiveMinuteIntervalForPricesInThePastDay()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var baseTime = DateTime.UtcNow;

            var instruments = new List<Instrument>
            {
                new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "")
            };

            var prices = new List<InstrumentPrice>
            {
                new InstrumentPrice(1, baseTime, baseTime,  100, 1),
                new InstrumentPrice(2, baseTime.AddMinutes(-5), baseTime, 100, 1),
                new InstrumentPrice(3, baseTime.AddMinutes(-6), baseTime, 100, 1),
                new InstrumentPrice(4, baseTime.AddMinutes(-10), baseTime, 100, 1),
                new InstrumentPrice(5, baseTime.AddHours(-23), baseTime, 100, 1),
                new InstrumentPrice(6, baseTime.AddHours(-23).AddMinutes(-3), baseTime, 100, 1),
                new InstrumentPrice(7, baseTime.AddHours(-23).AddMinutes(-5), baseTime, 100, 1)
            };

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(instruments);

            var priceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            priceRepository
                .Setup(m => m.ListInstrumentPricesAsync(instruments[0].Id))
                .ReturnsAsync(prices);

            var sut = fixture.Create<InstrumentPriceCleanupJob>();

            await sut.Run();

            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 1)), Times.Never());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 2)), Times.Never());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 3)), Times.Once());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 4)), Times.Never());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 5)), Times.Never());
            priceRepository.Verify(c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 6)), Times.Once());
        }
    }
}
