using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests;

public class ExchangesControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentService> _instrumentService;

    public ExchangesControllerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _instrumentService = _fixture.Freeze<Mock<IInstrumentService>>();
    }

    [Fact]
    public async Task GetKnownExchanges_ReturnsExchanges()
    {
        var exchanges = _fixture.CreateMany<ExchangeDto>();

        _instrumentService
            .Setup(m => m.GetKnownExchangesAsync())
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchanges));

        var sut = _fixture.Build<ExchangesController>().OmitAutoProperties().Create();

        var result = await sut.GetKnownExchanges();

        _instrumentService.Verify(m => m.GetKnownExchangesAsync(), Times.Once());
        Assert.Equal(exchanges, result.Value);
    }
}