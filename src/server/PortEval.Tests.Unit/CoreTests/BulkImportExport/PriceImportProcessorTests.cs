using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.Common.BulkImportExport;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.BulkImportExport
{
    public class PriceImportProcessorTests
    {
        [Fact]
        public async Task ProcessingImport_CreatesNewPrice_WhenPriceAtSameTimeDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Build<InstrumentPriceDto>()
                .With(p => p.InstrumentId, 1)
                .With(p => p.Time, DateTime.UtcNow)
                .Create();
            var priceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            priceService
                .Setup(m => m.GetInstrumentPriceAsync(price.InstrumentId, price.Time))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<InstrumentPriceDto>());

            var sut = fixture.Create<PriceImportProcessor>();

            await sut.ImportRecordsAsync(new List<InstrumentPriceDto> { price });

            priceService.Verify(s => s.AddPricePointAsync(It.Is<InstrumentPriceDto>(p =>
                p.InstrumentId == price.InstrumentId &&
                p.Time == price.Time &&
                p.Price == price.Price
            )), Times.Once());
        }

        [Fact]
        public async Task ProcessingImport_DoesNothing_WhenPriceAtSameTimeExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Build<InstrumentPriceDto>()
                .With(p => p.InstrumentId, 1)
                .With(p => p.Time, DateTime.UtcNow)
                .Create();
            var priceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            priceService
                .Setup(m => m.GetInstrumentPriceAsync(price.InstrumentId, price.Time))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(fixture.Create<InstrumentPriceDto>()));
            var sut = fixture.Create<PriceImportProcessor>();

            await sut.ImportRecordsAsync(new List<InstrumentPriceDto> { price });

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
            var priceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            var sut = fixture.Create<PriceImportProcessor>();

            await sut.ImportRecordsAsync(new List<InstrumentPriceDto> { price });

            priceService.Verify(s => s.AddPricePointAsync(It.IsAny<InstrumentPriceDto>()), Times.Never());
        }
    }
}