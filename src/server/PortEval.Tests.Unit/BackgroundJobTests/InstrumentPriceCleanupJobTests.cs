using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests;

public class InstrumentPriceCleanupJobTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private readonly Mock<IInstrumentPriceRepository> _priceRepository;

    public InstrumentPriceCleanupJobTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
        _priceRepository = _fixture.CreateDefaultInstrumentPriceRepositoryMock();
    }

    [Fact]
    public async Task Run_MaintainsOneDayIntervalForPricesOlderThan5Days()
    {
        var baseTime = DateTime.UtcNow;

        var instruments = new List<Instrument>
        {
            new(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "")
        };

        var prices = new List<InstrumentPrice>
        {
            new(1, baseTime.AddDays(-5), baseTime, 100, 1),
            new(2, baseTime.AddDays(-5).AddHours(-12), baseTime, 100, 1),
            new(3, baseTime.AddDays(-6), baseTime, 100, 1),
            new(4, baseTime.AddDays(-6).AddHours(-12), baseTime, 100, 1),
            new(5, baseTime.AddDays(-7), baseTime, 100, 1)
        };

        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(instruments);

        _priceRepository
            .Setup(m => m.ListInstrumentPricesAsync(instruments[0].Id))
            .ReturnsAsync(prices);

        var sut = _fixture.Create<InstrumentPriceCleanupJob>();

        await sut.RunAsync();

        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 1)),
            Times.Never());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 2)), Times.Once());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 3)),
            Times.Never());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 4)), Times.Once());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 5)),
            Times.Never());
    }

    [Fact]
    public async Task Run_MaintainsOneHourInterval_ForPricesInThePastFiveDays()
    {
        var baseTime = DateTime.UtcNow;

        var instruments = new List<Instrument>
        {
            new(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "")
        };

        var prices = new List<InstrumentPrice>
        {
            new(1, baseTime.AddDays(-1), baseTime, 100, 1),
            new(2, baseTime.AddDays(-1).AddMinutes(-30), baseTime, 100, 1),
            new(3, baseTime.AddDays(-1).AddHours(-1), baseTime, 100, 1),
            new(4, baseTime.AddDays(-4).AddHours(-23), baseTime, 100, 1),
            new(5, baseTime.AddDays(-4).AddHours(-23).AddMinutes(-30), baseTime, 100, 1),
            new(6, baseTime.AddDays(-5), baseTime, 100, 1)
        };

        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(instruments);

        _priceRepository
            .Setup(m => m.ListInstrumentPricesAsync(instruments[0].Id))
            .ReturnsAsync(prices);

        var sut = _fixture.Create<InstrumentPriceCleanupJob>();

        await sut.RunAsync();

        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 1)),
            Times.Never());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 2)), Times.Once());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 3)),
            Times.Never());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 4)),
            Times.Never());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 5)), Times.Once());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 6)),
            Times.Never());
    }

    [Fact]
    public async Task Run_MaintainsFiveMinuteIntervalForPricesInThePastDay()
    {
        var baseTime = DateTime.UtcNow;

        var instruments = new List<Instrument>
        {
            new(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "")
        };

        var prices = new List<InstrumentPrice>
        {
            new(1, baseTime, baseTime, 100, 1),
            new(2, baseTime.AddMinutes(-5), baseTime, 100, 1),
            new(3, baseTime.AddMinutes(-6), baseTime, 100, 1),
            new(4, baseTime.AddMinutes(-10), baseTime, 100, 1),
            new(5, baseTime.AddHours(-23), baseTime, 100, 1),
            new(6, baseTime.AddHours(-23).AddMinutes(-3), baseTime, 100, 1),
            new(7, baseTime.AddHours(-23).AddMinutes(-5), baseTime, 100, 1)
        };

        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(instruments);

        _priceRepository
            .Setup(m => m.ListInstrumentPricesAsync(instruments[0].Id))
            .ReturnsAsync(prices);

        var sut = _fixture.Create<InstrumentPriceCleanupJob>();

        await sut.RunAsync();

        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 1)),
            Times.Never());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 2)),
            Times.Never());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 3)), Times.Once());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 4)),
            Times.Never());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 5)),
            Times.Never());
        _priceRepository.Verify(
            c => c.Delete(It.Is<InstrumentPrice>(p => p.InstrumentId == instruments[0].Id && p.Id == 6)), Times.Once());
    }
}