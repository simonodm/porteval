using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.Common.BulkImportExport;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.BulkImportExport
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
            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            var sut = fixture.Create<PortfolioImportProcessor>();

            await sut.ImportRecordsAsync(new List<PortfolioDto> { portfolio });

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
            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            var sut = fixture.Create<PortfolioImportProcessor>();

            await sut.ImportRecordsAsync(new List<PortfolioDto> { portfolio });

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
            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            var sut = fixture.Create<PortfolioImportProcessor>();

            await sut.ImportRecordsAsync(new List<PortfolioDto> { portfolio });

            portfolioService.Verify(s => s.CreatePortfolioAsync(It.IsAny<PortfolioDto>()), Times.Never());
            portfolioService.Verify(s => s.UpdatePortfolioAsync(It.IsAny<PortfolioDto>()), Times.Never());
        }
    }
}