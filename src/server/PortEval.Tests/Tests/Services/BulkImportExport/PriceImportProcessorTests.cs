using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.BulkImportExport;
using PortEval.Tests.Extensions;
using Xunit;

namespace PortEval.Tests.Tests.Services.BulkImportExport
{
    public class PriceImportProcessorTests
    {
        [Fact]
        public async Task ProcessingImport_CreatesNewPrice_WhenNoIdIsPresent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Id, 0)
                .With(p => p.Time, DateTime.UtcNow)
                .Create();
            var priceService = fixture.CreateDefaultInstrumentPriceServiceMock();
            var sut = fixture.Create<PriceImportProcessor>();

            await sut.ImportRecords(new List<InstrumentPriceDto> { price });

            priceService.Verify(s => s.AddPricePointAsync(It.Is<InstrumentPriceDto>(p =>
                p.Id == default &&
                p.InstrumentId == price.InstrumentId &&
                p.Time == price.Time &&
                p.Price == price.Price
            )), Times.Once());
        }

        [Fact]
        public async Task ProcessingImport_DoesNothing_WhenIdIsPresent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, DateTime.UtcNow)
                .Create();
            var priceService = fixture.CreateDefaultInstrumentPriceServiceMock();
            var sut = fixture.Create<PriceImportProcessor>();

            await sut.ImportRecords(new List<InstrumentPriceDto> { price });

            priceService.Verify(s => s.AddPricePointAsync(It.IsAny<InstrumentPriceDto>()), Times.Never());
        }

        [Fact]
        public async Task ProcessingImport_DoesNothing_WhenPriceFailsValidation()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Build<InstrumentPriceDto>()
                .With(p => p.InstrumentId, 0)
                .Create();
            var priceService = fixture.CreateDefaultInstrumentPriceServiceMock();
            var sut = fixture.Create<PriceImportProcessor>();

            await sut.ImportRecords(new List<InstrumentPriceDto> { price });

            priceService.Verify(s => s.AddPricePointAsync(It.IsAny<InstrumentPriceDto>()), Times.Never());
        }
    }
}