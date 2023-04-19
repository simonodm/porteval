using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.DataFetcher.Responses;
using PortEval.Tests.Unit.DataFetcherTests.HelperTypes;
using Xunit;

namespace PortEval.Tests.Unit.DataFetcherTests;

public class RequestHandlerTests
{
    private readonly IFixture _fixture;

    public RequestHandlerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void Constructor_ThrowsException_WhenNoEligibleAPIsWereProvided()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var handler =
                new RequestHandler<TestRequest, bool>(
                    new TestRequest(), new List<DataSource>());
        });
    }

    [Fact]
    public async Task Handle_ReturnsApiResponse_WhenOneApiIsAvailable()
    {
        var request = new TestRequest();

        var api = CreateWorkingApiMock(_fixture);

        var sut = new RequestHandler<TestRequest, bool>(request,
            new List<DataSource> { api.Object });

        var response = await sut.HandleAsync();

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.True(response.Result);
    }

    [Fact]
    public async Task Handle_ReturnsSuccessfulApiResponse_WhenOneWorkingAndMultipleFailingApisAreProvided()
    {
        var request = new TestRequest();

        var workingApi = CreateWorkingApiMock(_fixture);

        var firstFailingApi = CreateUnresponsiveApiMock(_fixture);
        var secondFailingApi = CreateUnresponsiveApiMock(_fixture);
        var thirdFailingApi = CreateFailingApiMock(_fixture);

        var sut = new RequestHandler<TestRequest, bool>(request,
            new List<DataSource>
                { firstFailingApi.Object, secondFailingApi.Object, workingApi.Object, thirdFailingApi.Object });

        var response = await sut.HandleAsync();

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.True(response.Result);
    }

    [Fact]
    public async Task Handle_ReturnsOtherError_WhenNoApiWasAbleToProcessTheRequest()
    {
        var request = new TestRequest();

        var firstFailingApi = CreateUnresponsiveApiMock(_fixture);
        var secondFailingApi = CreateUnresponsiveApiMock(_fixture);
        var thirdFailingApi = CreateFailingApiMock(_fixture);

        var sut = new RequestHandler<TestRequest, bool>(
            request,
            new List<DataSource> { firstFailingApi.Object, secondFailingApi.Object, thirdFailingApi.Object },
            RetryPolicy.None);

        var response = await sut.HandleAsync();

        Assert.Equal(StatusCode.OtherError, response.StatusCode);
        Assert.NotEmpty(response.ErrorMessage);
    }

    [Fact]
    public async Task Handle_ReturnsSuccessfulResponse_WhenRetryResultsInASuccessfulResponse()
    {
        var request = new TestRequest();
        var retryInterval = TimeSpan.Zero;
        var retryPolicy = new RetryPolicy
        {
            RetryIntervals = { retryInterval }
        };

        var api = CreateTemporarilyFailingApiMock(_fixture);

        var sut = new RequestHandler<TestRequest, bool>(
            request,
            new List<DataSource> { api.Object },
            retryPolicy);

        var response = await sut.HandleAsync();

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.True(response.Result);
    }

    private Mock<TestFinancialApi> CreateWorkingApiMock(IFixture fixture)
    {
        var api = fixture.Create<Mock<TestFinancialApi>>();
        api
            .Setup(m => m.Process(It.IsAny<TestRequest>()))
            .ReturnsAsync(new Response<bool>
            {
                StatusCode = StatusCode.Ok,
                Result = true
            });

        return api;
    }

    private Mock<TestFinancialApi> CreateTemporarilyFailingApiMock(IFixture fixture)
    {
        var api = fixture.Create<Mock<TestFinancialApi>>();
        api
            .SetupSequence(m => m.Process(It.IsAny<TestRequest>()))
            .ReturnsAsync(new Response<bool>
            {
                StatusCode = StatusCode.ConnectionError,
                ErrorMessage = "Connection error."
            })
            .ReturnsAsync(new Response<bool>
            {
                StatusCode = StatusCode.Ok,
                Result = true
            });

        return api;
    }

    private Mock<TestFinancialApi> CreateFailingApiMock(IFixture fixture)
    {
        var api = fixture.Create<Mock<TestFinancialApi>>();
        api
            .Setup(m => m.Process(It.IsAny<TestRequest>()))
            .ReturnsAsync(new Response<bool>
            {
                StatusCode = StatusCode.OtherError,
                ErrorMessage = "Internal error."
            });

        return api;
    }

    private Mock<TestFinancialApi> CreateUnresponsiveApiMock(IFixture fixture)
    {
        var api = fixture.Create<Mock<TestFinancialApi>>();
        api
            .Setup(m => m.Process(It.IsAny<TestRequest>()))
            .ReturnsAsync(new Response<bool>
            {
                StatusCode = StatusCode.ConnectionError,
                ErrorMessage = "Failed to connect."
            });

        return api;
    }
}