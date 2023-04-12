using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class InstrumentSplitsControllerTests
    {
        private IFixture _fixture;
        private Mock<IInstrumentSplitService> _instrumentSplitService;

        public InstrumentSplitsControllerTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _instrumentSplitService = _fixture.Freeze<Mock<IInstrumentSplitService>>();
        }

        [Fact]
        public async Task GetInstrumentSplits_ReturnsSplitsOfAnInstrument_WhenInstrumentExists()
        {
            var instrumentId = _fixture.Create<int>();
            var split = _fixture.Create<InstrumentSplitDto>();

            _instrumentSplitService
                .Setup(m => m.GetInstrumentSplitsAsync(instrumentId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse<IEnumerable<InstrumentSplitDto>>(new[] { split }));

            var sut = _fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentSplits(instrumentId);

            _instrumentSplitService.Verify(m => m.GetInstrumentSplitsAsync(instrumentId));
            Assert.Collection(result.Value, s =>
            {
                Assert.Equal(split, s);
            });
        }

        [Fact]
        public async Task GetInstrumentSplits_ReturnsNotFound_WhenQueryReturnsNotFound()
        {
            var instrumentId = _fixture.Create<int>();
            var split = _fixture.Create<InstrumentSplitDto>();

            _instrumentSplitService
                .Setup(m => m.GetInstrumentSplitsAsync(instrumentId))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<InstrumentSplitDto>>());

            var sut = _fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentSplits(instrumentId);

            _instrumentSplitService.Verify(m => m.GetInstrumentSplitsAsync(instrumentId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInstrumentSplit_ReturnsInstrumentSplit_WhenItExists()
        {
            var split = _fixture.Create<InstrumentSplitDto>();

            _instrumentSplitService
                .Setup(m => m.GetInstrumentSplitAsync(split.InstrumentId, split.Id))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(split));

            var sut = _fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentSplit(split.InstrumentId, split.Id);

            _instrumentSplitService.Verify(m => m.GetInstrumentSplitAsync(split.InstrumentId, split.Id));
            Assert.Equal(split, result.Value);
        }

        [Fact]
        public async Task PostInstrumentSplit_CreatesSplit()
        {
            var split = _fixture.Create<InstrumentSplitDto>();

            _instrumentSplitService
                .Setup(m => m.CreateSplitAsync(split))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(split));

            var sut = _fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            await sut.PostInstrumentSplit(split.InstrumentId, split);

            _instrumentSplitService.Verify(m => m.CreateSplitAsync(split));
        }

        [Fact]
        public async Task PostInstrumentSplit_ReturnsBadRequest_WhenQueryParamInstrumentIdAndBodyInstrumentIdDontMatch()
        {
            var split = _fixture.Create<InstrumentSplitDto>();
            var sut = _fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.PostInstrumentSplit(split.InstrumentId + 1, split);

            _instrumentSplitService.Verify(m => m.CreateSplitAsync(It.IsAny<InstrumentSplitDto>()), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutInstrumentSplit_UpdatesSplit()
        {
            var split = _fixture.Create<InstrumentSplitDto>();

            _instrumentSplitService
                .Setup(m => m.CreateSplitAsync(split))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(split));
            _instrumentSplitService
                .Setup(m => m.UpdateSplitAsync(split.InstrumentId, split))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(split));

            var sut = _fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            await sut.PutInstrumentSplit(split.InstrumentId, split);

            _instrumentSplitService.Verify(m => m.UpdateSplitAsync(split.InstrumentId, split));
        }

        [Fact]
        public async Task PutInstrumentSplit_ReturnsBadRequest_WhenQueryParamInstrumentIdAndBodyInstrumentIdDontMatch()
        {
            var split = _fixture.Create<InstrumentSplitDto>();
            var sut = _fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.PutInstrumentSplit(split.InstrumentId + 1, split);

            _instrumentSplitService.Verify(m => m.UpdateSplitAsync(It.IsAny<int>(), It.IsAny<InstrumentSplitDto>()), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }
    }
}
