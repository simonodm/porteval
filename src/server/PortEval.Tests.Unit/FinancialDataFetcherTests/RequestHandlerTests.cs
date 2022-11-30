using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.FinancialDataFetcher;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Responses;
using PortEval.Tests.Unit.FinancialDataFetcherTests.HelperTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.FinancialDataFetcherTests
{
    public class RequestHandlerTests
    {
        [Fact]
        public void Constructor_ThrowsException_WhenNoEligibleAPIsWereProvided()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var handler =
                    new RequestHandler<ITestFinancialApi, TestRequest, Response<bool>>(
                        new TestRequest(), new List<ITestFinancialApi>());
            });
        }

        [Fact]
        public async Task Handle_ReturnsApiResponse_WhenOneApiIsAvailable()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var request = new TestRequest();

            var api = CreateWorkingApiMock(fixture);

            var sut = new RequestHandler<ITestFinancialApi, TestRequest, Response<bool>>(request,
                new List<ITestFinancialApi> { api.Object });

            var response = await sut.Handle();

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessfulApiResponse_WhenOneWorkingAndMultipleFailingApisAreProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var request = new TestRequest();

            var workingApi = CreateWorkingApiMock(fixture);

            var firstFailingApi = CreateUnresponsiveApiMock(fixture);
            var secondFailingApi = CreateUnresponsiveApiMock(fixture);
            var thirdFailingApi = CreateFailingApiMock(fixture);

            var sut = new RequestHandler<ITestFinancialApi, TestRequest, Response<bool>>(request,
                new List<ITestFinancialApi> { firstFailingApi.Object, secondFailingApi.Object, workingApi.Object, thirdFailingApi.Object });

            var response = await sut.Handle();

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task Handle_ReturnsOtherError_WhenNoApiWasAbleToProcessTheRequest()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var request = new TestRequest();

            var firstFailingApi = CreateUnresponsiveApiMock(fixture);
            var secondFailingApi = CreateUnresponsiveApiMock(fixture);
            var thirdFailingApi = CreateFailingApiMock(fixture);

            var sut = new RequestHandler<ITestFinancialApi, TestRequest, Response<bool>>(
                request,
                new List<ITestFinancialApi> { firstFailingApi.Object, secondFailingApi.Object, thirdFailingApi.Object },
                RetryPolicy.None);

            var response = await sut.Handle();

            Assert.Equal(StatusCode.OtherError, response.StatusCode);
            Assert.NotEmpty(response.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessfulResponse_WhenRetryResultsInASuccessfulResponse()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var request = new TestRequest();
            var retryInterval = TimeSpan.Zero;
            var retryPolicy = new RetryPolicy
            {
                RetryIntervals = { retryInterval }
            };

            var api = CreateTemporarilyFailingApiMock(fixture);

            var sut = new RequestHandler<ITestFinancialApi, TestRequest, Response<bool>>(
                request,
                new List<ITestFinancialApi> { api.Object },
                retryPolicy);

            var response = await sut.Handle();

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.True(response.Result);
        }

        private Mock<ITestFinancialApi> CreateWorkingApiMock(IFixture fixture)
        {
            var api = fixture.Create<Mock<ITestFinancialApi>>();
            api
                .Setup(m => m.Process(It.IsAny<TestRequest>()))
                .ReturnsAsync(new Response<bool>
                {
                    StatusCode = StatusCode.Ok,
                    Result = true
                });

            return api;
        }

        private Mock<ITestFinancialApi> CreateTemporarilyFailingApiMock(IFixture fixture)
        {
            int counter = 0;
            var api = fixture.Create<Mock<ITestFinancialApi>>();
            api
                .SetupSequence(m => m.Process(It.IsAny<TestRequest>()))
                .ReturnsAsync(new Response<bool>
                {
                    StatusCode = StatusCode.ConnectionError,
                    ErrorMessage = "Connection error."
                })
                .ReturnsAsync(new Response<bool>()
                {
                    StatusCode = StatusCode.Ok,
                    Result = true
                });

            return api;
        }

        private Mock<ITestFinancialApi> CreateFailingApiMock(IFixture fixture)
        {
            var api = fixture.Create<Mock<ITestFinancialApi>>();
            api
                .Setup(m => m.Process(It.IsAny<TestRequest>()))
                .ReturnsAsync(new Response<bool>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "Internal error."
                });

            return api;
        }

        private Mock<ITestFinancialApi> CreateUnresponsiveApiMock(IFixture fixture)
        {
            var api = fixture.Create<Mock<ITestFinancialApi>>();
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
}
