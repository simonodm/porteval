using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.Common.BulkImport;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.BulkImportExport;

public class PriceImportProcessorTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentPriceService> _priceService;

    public PriceImportProcessorTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _priceService = _fixture.Freeze<Mock<IInstrumentPriceService>>();
    }

    [Fact]
    public async Task ProcessingImport_CreatesNewPrice_WhenPriceAtSameTimeDoesNotExist()
    {
        var price = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.InstrumentId, 1)
            .With(p => p.Time, DateTime.UtcNow)
            .Create();
        _priceService
            .Setup(m => m.GetInstrumentPriceAsync(price.InstrumentId, price.Time))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<InstrumentPriceDto>());

        var sut = _fixture.Create<PriceImportProcessor>();

        await sut.ImportRecordsAsync(new List<InstrumentPriceDto> { price });

        _priceService.Verify(s => s.AddPricePointAsync(It.Is<InstrumentPriceDto>(p =>
            p.InstrumentId == price.InstrumentId &&
            p.Time == price.Time &&
            p.Price == price.Price
        )), Times.Once());
    }

    [Fact]
    public async Task ProcessingImport_DoesNothing_WhenPriceAtSameTimeExists()
    {
        var price = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.InstrumentId, 1)
            .With(p => p.Time, DateTime.UtcNow)
            .Create();
        _priceService
            .Setup(m => m.GetInstrumentPriceAsync(price.InstrumentId, price.Time))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(price));
        var sut = _fixture.Create<PriceImportProcessor>();

        await sut.ImportRecordsAsync(new List<InstrumentPriceDto> { price });

        _priceService.Verify(s => s.AddPricePointAsync(It.IsAny<InstrumentPriceDto>()), Times.Never());
    }

    [Fact]
    public async Task ProcessingImport_DoesNothing_WhenPriceFailsValidation()
    {
        var price = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.InstrumentId, 0)
            .Create();
        var sut = _fixture.Create<PriceImportProcessor>();

        await sut.ImportRecordsAsync(new List<InstrumentPriceDto> { price });

        _priceService.Verify(s => s.AddPricePointAsync(It.IsAny<InstrumentPriceDto>()), Times.Never());
    }
}