using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.Common.BulkImportExport;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.BulkImportExport
{
    public class PortfolioImportProcessorTests
    {
        private IFixture _fixture;
        private Mock<IPortfolioService> _portfolioService;

        public PortfolioImportProcessorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _portfolioService = _fixture.Freeze<Mock<IPortfolioService>>();
        }

        [Fact]
        public async Task ProcessingImport_CreatesNewPortfolio_WhenNoIdIsPresent()
        {
            var portfolio = _fixture
                .Build<PortfolioDto>()
                .With(p => p.Id, 0)
                .With(p => p.CurrencyCode, "USD")
                .Create();
            var sut = _fixture.Create<PortfolioImportProcessor>();

            await sut.ImportRecordsAsync(new List<PortfolioDto> { portfolio });

            _portfolioService.Verify(s => s.CreatePortfolioAsync(It.Is<PortfolioDto>(p =>
                p.Id == default &&
                p.Name == portfolio.Name &&
                p.CurrencyCode == portfolio.CurrencyCode &&
                p.Note == portfolio.Note
            )));
        }

        [Fact]
        public async Task ProcessingImport_UpdatesPortfolio_WhenIdIsPresent()
        {
            var portfolio = _fixture
                .Build<PortfolioDto>()
                .With(p => p.CurrencyCode, "USD")
                .Create();
            var sut = _fixture.Create<PortfolioImportProcessor>();

            await sut.ImportRecordsAsync(new List<PortfolioDto> { portfolio });

            _portfolioService.Verify(s => s.UpdatePortfolioAsync(It.Is<PortfolioDto>(p =>
                p.Id == portfolio.Id &&
                p.Name == portfolio.Name &&
                p.CurrencyCode == portfolio.CurrencyCode &&
                p.Note == portfolio.Note
            )));
        }

        [Fact]
        public async Task ProcessingImport_DoesNothing_WhenPortfolioFailsValidation()
        {
            var portfolio = _fixture.Create<PortfolioDto>();
            var sut = _fixture.Create<PortfolioImportProcessor>();

            await sut.ImportRecordsAsync(new List<PortfolioDto> { portfolio });

            _portfolioService.Verify(s => s.CreatePortfolioAsync(It.IsAny<PortfolioDto>()), Times.Never());
            _portfolioService.Verify(s => s.UpdatePortfolioAsync(It.IsAny<PortfolioDto>()), Times.Never());
        }
    }
}