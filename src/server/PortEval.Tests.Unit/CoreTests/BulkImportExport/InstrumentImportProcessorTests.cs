using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.Common.BulkImport;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.BulkImportExport;

public class InstrumentImportProcessorTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentService> _instrumentService;

    public InstrumentImportProcessorTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _instrumentService = _fixture.Freeze<Mock<IInstrumentService>>();
    }

    [Fact]
    public async Task ProcessingImport_CreatesNewInstrument_WhenNoIdIsPresent()
    {
        var instrument = _fixture.Build<InstrumentDto>()
            .With(i => i.Symbol, "AAPL")
            .With(i => i.CurrencyCode, "USD")
            .With(i => i.Exchange, "NASDAQ")
            .With(i => i.Id, 0)
            .Create();
        var sut = _fixture.Create<InstrumentImportProcessor>();

        await sut.ImportRecordsAsync(new List<InstrumentDto> { instrument });

        _instrumentService.Verify(s => s.CreateInstrumentAsync(It.Is<InstrumentDto>(i =>
            i.Id == default &&
            i.Name == instrument.Name &&
            i.Symbol == instrument.Symbol &&
            i.CurrencyCode == instrument.CurrencyCode &&
            i.Type == instrument.Type &&
            i.Exchange == instrument.Exchange
        )), Times.Once());
    }

    [Fact]
    public async Task ProcessingImport_UpdatesInstrument_WhenIdIsPresent()
    {
        var instrument = _fixture.Build<InstrumentDto>()
            .With(i => i.Symbol, "AAPL")
            .With(i => i.CurrencyCode, "USD")
            .With(i => i.Exchange, "NASDAQ")
            .Create();
        var sut = _fixture.Create<InstrumentImportProcessor>();

        await sut.ImportRecordsAsync(new List<InstrumentDto> { instrument });

        _instrumentService.Verify(s => s.UpdateInstrumentAsync(It.Is<InstrumentDto>(i =>
            i.Id == instrument.Id &&
            i.Name == instrument.Name &&
            i.Symbol == instrument.Symbol &&
            i.CurrencyCode == instrument.CurrencyCode &&
            i.Type == instrument.Type &&
            i.Exchange == instrument.Exchange
        )), Times.Once());
    }

    [Fact]
    public async Task ProcessingImport_DoesNothing_WhenInstrumentFailsValidation()
    {
        var instrument = _fixture.Build<InstrumentDto>()
            .With(i => i.Symbol, "")
            .With(i => i.CurrencyCode, "")
            .Create();
        var sut = _fixture.Create<InstrumentImportProcessor>();

        await sut.ImportRecordsAsync(new List<InstrumentDto> { instrument });

        _instrumentService.Verify(s => s.CreateInstrumentAsync(It.IsAny<InstrumentDto>()), Times.Never());
        _instrumentService.Verify(s => s.UpdateInstrumentAsync(It.IsAny<InstrumentDto>()), Times.Never());
    }
}