using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class InstrumentPricesControllerTests
    {
        [Fact]
        public async Task GetInstrumentPriceAt_ReturnsInstrumentPrice_WhenPriceExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var price = fixture.Create<InstrumentPriceDto>();

            var instrumentPriceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            instrumentPriceService
                .Setup(m => m.GetInstrumentPriceAsync(instrumentId, price.Time))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(price));

            var sut = fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentPriceAt(instrumentId, price.Time);

            instrumentPriceService.Verify(m => m.GetInstrumentPriceAsync(instrumentId, price.Time));
            Assert.Equal(price, result.Value);
        }

        [Fact]
        public async Task GetInstrumentPriceAt_ReturnsNotFound_WhenQueryReturnsNotFound()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentPriceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            instrumentPriceService
                .Setup(m => m.GetInstrumentPriceAsync(instrumentId, It.IsAny<DateTime>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<InstrumentPriceDto>());

            var sut = fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentPriceAt(instrumentId, DateTime.UtcNow);

            instrumentPriceService.Verify(m => m.GetInstrumentPriceAsync(instrumentId, It.IsAny<DateTime>()));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetLatestInstrumentPrice_ReturnsCurrentPrice_WhenPriceExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var now = DateTime.UtcNow;
            var price = fixture.Build<InstrumentPriceDto>().With(p => p.Time, now).Create();

            var instrumentPriceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            instrumentPriceService
                .Setup(m => m.GetInstrumentPriceAsync(instrumentId, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(price));

            var sut = fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestInstrumentPrice(instrumentId);

            instrumentPriceService.Verify(m => m.GetInstrumentPriceAsync(instrumentId, It.Is<DateTime>(dt => dt >= now)));
            Assert.Equal(price, result.Value);
        }

        [Fact]
        public async Task GetLatestInstrumentPrice_ReturnsNotFound_WhenPriceDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentPriceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            instrumentPriceService
                .Setup(m => m.GetInstrumentPriceAsync(instrumentId, It.IsAny<DateTime>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<InstrumentPriceDto>());

            var sut = fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestInstrumentPrice(instrumentId);

            instrumentPriceService.Verify(m => m.GetInstrumentPriceAsync(instrumentId, It.IsAny<DateTime>()));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostPricePoint_CreatesPrice()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, DateTime.UtcNow)
                .Create();

            var instrumentPriceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            instrumentPriceService
                .Setup(m => m.AddPricePointAsync(price))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(price));

            var sut = fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

            await sut.PostPricePoint(price.InstrumentId, price);

            instrumentPriceService.Verify(m => m.AddPricePointAsync(price));
        }

        [Fact]
        public async Task PostPricePoint_ReturnsBadRequest_WhenQueryParamInstrumentIdAndBodyInstrumentIdDontMatch()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Create<InstrumentPriceDto>();

            var instrumentPriceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            instrumentPriceService
                .Setup(m => m.AddPricePointAsync(price))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(price));

            var sut = fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

            var result = await sut.PostPricePoint(price.InstrumentId + 1, price);

            instrumentPriceService.Verify(m => m.AddPricePointAsync(price), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeletePricePoint_DeletesPrice()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var priceId = fixture.Create<int>();

            var instrumentPriceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            instrumentPriceService
                .Setup(m => m.DeletePricePointByIdAsync(instrumentId, priceId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());

            var sut = fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

            await sut.DeletePricePoint(instrumentId, priceId);

            instrumentPriceService.Verify(m => m.DeletePricePointByIdAsync(instrumentId, priceId));
        }
    }
}
