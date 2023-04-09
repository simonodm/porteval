using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using PortEval.Application.Core.Common.BulkImportExport;

namespace PortEval.Tests.Unit.FeatureTests.BulkImportExport
{
    public class InstrumentImportProcessorTests
    {
        [Fact]
        public async Task ProcessingImport_CreatesNewInstrument_WhenNoIdIsPresent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Build<InstrumentDto>()
                .With(i => i.Symbol, "AAPL")
                .With(i => i.CurrencyCode, "USD")
                .With(i => i.Exchange, "NASDAQ")
                .With(i => i.Id, 0)
                .Create();
            var instrumentService = fixture.CreateDefaultInstrumentServiceMock();
            var sut = fixture.Create<InstrumentImportProcessor>();

            await sut.ImportRecordsAsync(new List<InstrumentDto> { instrument });

            instrumentService.Verify(s => s.CreateInstrumentAsync(It.Is<InstrumentDto>(i =>
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
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Build<InstrumentDto>()
                .With(i => i.Symbol, "AAPL")
                .With(i => i.CurrencyCode, "USD")
                .With(i => i.Exchange, "NASDAQ")
                .Create();
            var instrumentService = fixture.CreateDefaultInstrumentServiceMock();
            var sut = fixture.Create<InstrumentImportProcessor>();

            await sut.ImportRecordsAsync(new List<InstrumentDto> { instrument });

            instrumentService.Verify(s => s.UpdateInstrumentAsync(It.Is<InstrumentDto>(i =>
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
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Build<InstrumentDto>()
                .With(i => i.Symbol, "")
                .With(i => i.CurrencyCode, "")
                .Create();
            var instrumentService = fixture.CreateDefaultInstrumentServiceMock();
            var sut = fixture.Create<InstrumentImportProcessor>();

            await sut.ImportRecordsAsync(new List<InstrumentDto> { instrument });

            instrumentService.Verify(s => s.CreateInstrumentAsync(It.IsAny<InstrumentDto>()), Times.Never());
            instrumentService.Verify(s => s.UpdateInstrumentAsync(It.IsAny<InstrumentDto>()), Times.Never());
        }
    }
}