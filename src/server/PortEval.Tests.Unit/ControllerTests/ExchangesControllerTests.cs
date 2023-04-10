using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class ExchangesControllerTests
    {
        [Fact]
        public async Task GetKnownExchanges_ReturnsExchanges()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchanges = fixture.CreateMany<ExchangeDto>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.GetKnownExchangesAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchanges));

            var sut = fixture.Build<ExchangesController>().OmitAutoProperties().Create();

            var result = await sut.GetKnownExchanges();

            instrumentService.Verify(m => m.GetKnownExchangesAsync(), Times.Once());
            Assert.Equal(exchanges, result.Value);
        }
    }
}
