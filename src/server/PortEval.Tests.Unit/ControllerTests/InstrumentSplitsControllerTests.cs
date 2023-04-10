using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class InstrumentSplitsControllerTests
    {
        [Fact]
        public async Task GetInstrumentSplits_ReturnsSplitsOfAnInstrument_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var split = fixture.Create<InstrumentSplitDto>();

            var instrumentSplitService = fixture.Freeze<Mock<IInstrumentSplitService>>();
            instrumentSplitService
                .Setup(m => m.GetInstrumentSplitsAsync(instrumentId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse<IEnumerable<InstrumentSplitDto>>(new[] { split }));

            var sut = fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentSplits(instrumentId);

            instrumentSplitService.Verify(m => m.GetInstrumentSplitsAsync(instrumentId));
            Assert.Collection(result.Value, s =>
            {
                Assert.Equal(split, s);
            });
        }

        [Fact]
        public async Task GetInstrumentSplits_ReturnsNotFound_WhenQueryReturnsNotFound()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var split = fixture.Create<InstrumentSplitDto>();

            var instrumentSplitService = fixture.Freeze<Mock<IInstrumentSplitService>>();
            instrumentSplitService
                .Setup(m => m.GetInstrumentSplitsAsync(instrumentId))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<InstrumentSplitDto>>());

            var sut = fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentSplits(instrumentId);

            instrumentSplitService.Verify(m => m.GetInstrumentSplitsAsync(instrumentId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInstrumentSplit_ReturnsInstrumentSplit_WhenItExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            var split = fixture.Create<InstrumentSplitDto>();

            var instrumentSplitService = fixture.Freeze<Mock<IInstrumentSplitService>>();
            instrumentSplitService
                .Setup(m => m.GetInstrumentSplitAsync(split.InstrumentId, split.Id))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(split));

            var sut = fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentSplit(split.InstrumentId, split.Id);

            instrumentSplitService.Verify(m => m.GetInstrumentSplitAsync(split.InstrumentId, split.Id));
            Assert.Equal(split, result.Value);
        }

        [Fact]
        public async Task PostInstrumentSplit_CreatesSplit()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var split = fixture.Create<InstrumentSplitDto>();

            var instrumentSplitService = fixture.Freeze<Mock<IInstrumentSplitService>>();
            instrumentSplitService
                .Setup(m => m.CreateSplitAsync(split))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(split));

            var sut = fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            await sut.PostInstrumentSplit(split.InstrumentId, split);

            instrumentSplitService.Verify(m => m.CreateSplitAsync(split));
        }

        [Fact]
        public async Task PostInstrumentSplit_ReturnsBadRequest_WhenQueryParamInstrumentIdAndBodyInstrumentIdDontMatch()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var split = fixture.Create<InstrumentSplitDto>();
            var instrumentSplitService = fixture.Freeze<Mock<IInstrumentSplitService>>();

            var sut = fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.PostInstrumentSplit(split.InstrumentId + 1, split);

            instrumentSplitService.Verify(m => m.CreateSplitAsync(It.IsAny<InstrumentSplitDto>()), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutInstrumentSplit_UpdatesSplit()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var split = fixture.Create<InstrumentSplitDto>();

            var instrumentSplitService = fixture.Freeze<Mock<IInstrumentSplitService>>();
            instrumentSplitService
                .Setup(m => m.CreateSplitAsync(split))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(split));
            instrumentSplitService
                .Setup(m => m.UpdateSplitAsync(split.InstrumentId, split))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(split));

            var sut = fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            await sut.PutInstrumentSplit(split.InstrumentId, split);

            instrumentSplitService.Verify(m => m.UpdateSplitAsync(split.InstrumentId, split));
        }

        [Fact]
        public async Task PutInstrumentSplit_ReturnsBadRequest_WhenQueryParamInstrumentIdAndBodyInstrumentIdDontMatch()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var split = fixture.Create<InstrumentSplitDto>();
            var instrumentSplitService = fixture.Freeze<Mock<IInstrumentSplitService>>();

            var sut = fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.PutInstrumentSplit(split.InstrumentId + 1, split);

            instrumentSplitService.Verify(m => m.UpdateSplitAsync(It.IsAny<int>(), It.IsAny<InstrumentSplitDto>()), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }
    }
}
