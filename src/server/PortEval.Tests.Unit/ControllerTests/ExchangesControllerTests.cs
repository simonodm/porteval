﻿using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Tests.Unit.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class ExchangesControllerTests
    {
        [Fact]
        public async Task GetKnownExchanges_ReturnsExchangesFromQuery()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchanges = fixture.CreateMany<ExchangeDto>();
            
            var exchangeQueries = fixture.Freeze<Mock<IExchangeQueries>>();
            exchangeQueries
                .Setup(m => m.GetKnownExchanges())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(exchanges));

            var sut = fixture.Build<ExchangesController>().OmitAutoProperties().Create();

            var result = await sut.GetKnownExchanges();

            exchangeQueries.Verify(m => m.GetKnownExchanges(), Times.Once());
            Assert.Equal(exchanges, result.Value);
        }
    }
}
