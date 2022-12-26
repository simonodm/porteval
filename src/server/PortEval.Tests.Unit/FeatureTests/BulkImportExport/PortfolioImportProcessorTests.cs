using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Services.BulkImportExport;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.BulkImportExport
{
    public class PortfolioImportProcessorTests
    {
        [Fact]
        public async Task ProcessingImport_CreatesNewPortfolio_WhenNoIdIsPresent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture
                .Build<PortfolioDto>()
                .With(p => p.Id, 0)
                .With(p => p.CurrencyCode, "USD")
                .Create();
            var portfolioService = fixture.CreateDefaultPortfolioServiceMock();
            var sut = fixture.Create<PortfolioImportProcessor>();

            await sut.ImportRecords(new List<PortfolioDto> { portfolio });

            portfolioService.Verify(s => s.CreatePortfolioAsync(It.Is<PortfolioDto>(p =>
                p.Id == default &&
                p.Name == portfolio.Name &&
                p.CurrencyCode == portfolio.CurrencyCode &&
                p.Note == portfolio.Note
            )));
        }

        [Fact]
        public async Task ProcessingImport_UpdatesPortfolio_WhenIdIsPresent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture
                .Build<PortfolioDto>()
                .With(p => p.CurrencyCode, "USD")
                .Create();
            var portfolioService = fixture.CreateDefaultPortfolioServiceMock();
            var sut = fixture.Create<PortfolioImportProcessor>();

            await sut.ImportRecords(new List<PortfolioDto> { portfolio });

            portfolioService.Verify(s => s.UpdatePortfolioAsync(It.Is<PortfolioDto>(p =>
                p.Id == portfolio.Id &&
                p.Name == portfolio.Name &&
                p.CurrencyCode == portfolio.CurrencyCode &&
                p.Note == portfolio.Note
            )));
        }

        [Fact]
        public async Task ProcessingImport_DoesNothing_WhenPortfolioFailsValidation()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Create<PortfolioDto>();
            var portfolioService = fixture.CreateDefaultPortfolioServiceMock();
            var sut = fixture.Create<PortfolioImportProcessor>();

            await sut.ImportRecords(new List<PortfolioDto> { portfolio });

            portfolioService.Verify(s => s.CreatePortfolioAsync(It.IsAny<PortfolioDto>()), Times.Never());
            portfolioService.Verify(s => s.UpdatePortfolioAsync(It.IsAny<PortfolioDto>()), Times.Never());
        }
    }
}