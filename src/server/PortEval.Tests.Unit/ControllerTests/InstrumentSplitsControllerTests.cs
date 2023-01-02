using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Domain.Models.Entities;
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentSplits(instrumentId))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse<IEnumerable<InstrumentSplitDto>>(new [] { split }));

            var sut = fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentSplits(instrumentId);

            instrumentQueries.Verify(m => m.GetInstrumentSplits(instrumentId));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentSplits(instrumentId))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<InstrumentSplitDto>>());

            var sut = fixture.Build<InstrumentSplitsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentSplits(instrumentId);

            instrumentQueries.Verify(m => m.GetInstrumentSplits(instrumentId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
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
                .ReturnsAsync(fixture.Create<InstrumentSplit>());

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
                .ReturnsAsync(fixture.Create<InstrumentSplit>());

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
