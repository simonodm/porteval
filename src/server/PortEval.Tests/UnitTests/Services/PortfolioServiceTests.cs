using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Extensions;
using Xunit;

namespace PortEval.Tests.UnitTests.Services
{
    public class PortfolioServiceTests
    {
        [Fact]
        public async Task CreatingPortfolio_AddsPortfolioToRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<PortfolioService>();

            await sut.CreatePortfolioAsync(portfolioDto);

            portfolioRepository.Verify(r => r.Add(It.Is<Portfolio>(p =>
                p.Name == portfolioDto.Name &&
                p.Note == portfolioDto.Note &&
                p.CurrencyCode == portfolioDto.CurrencyCode
            )), Times.Once());
        }

        [Fact]
        public async Task CreatingPortfolio_ThrowsException_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.Exists(portfolioDto.CurrencyCode))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<PortfolioService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.CreatePortfolioAsync(portfolioDto));
        }

        [Fact]
        public async Task CreatingPortfolio_ReturnsNewPortfolio_WhenCreatedSuccessfully()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<PortfolioService>();

            var createdPortfolio = await sut.CreatePortfolioAsync(portfolioDto);

            Assert.Equal(portfolioDto.Name, createdPortfolio.Name);
            Assert.Equal(portfolioDto.CurrencyCode, createdPortfolio.CurrencyCode);
            Assert.Equal(portfolioDto.Note, createdPortfolio.Note);
        }

        [Fact]
        public async Task UpdatingPortfolio_UpdatesPortfolio_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<PortfolioService>();

            await sut.UpdatePortfolioAsync(portfolioDto);

            portfolioRepository.Verify(r => r.Update(It.Is<Portfolio>(p =>
                p.Name == portfolioDto.Name &&
                p.Note == portfolioDto.Note &&
                p.CurrencyCode == portfolioDto.CurrencyCode
            )), Times.Once());
        }

        [Fact]
        public async Task UpdatingPortfolio_ThrowsException_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.Exists(portfolioDto.CurrencyCode))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<PortfolioService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdatePortfolioAsync(portfolioDto));
        }

        [Fact]
        public async Task UpdatingPortfolio_ThrowsException_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            portfolioRepository
                .Setup(r => r.Exists(portfolioDto.Id))
                .Returns(Task.FromResult(false));
            portfolioRepository
                .Setup(r => r.FindAsync(portfolioDto.Id))
                .Returns(Task.FromResult<Portfolio>(null));
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<PortfolioService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdatePortfolioAsync(portfolioDto));
        }

        [Fact]
        public async Task UpdatingPortfolio_ReturnsNewPortfolio_WhenUpdatedSuccessfully()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<PortfolioService>();

            var updatedPortfolio = await sut.UpdatePortfolioAsync(portfolioDto);

            Assert.Equal(portfolioDto.Name, updatedPortfolio.Name);
            Assert.Equal(portfolioDto.CurrencyCode, updatedPortfolio.CurrencyCode);
            Assert.Equal(portfolioDto.Note, updatedPortfolio.Note);
        }

        [Fact]
        public async Task DeletingPortfolio_DeletesPortfolio_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<PortfolioService>();

            await sut.DeletePortfolioAsync(portfolioId);

            portfolioRepository.Verify(r => r.Delete(portfolioId), Times.Once());
        }

        [Fact]
        public async Task DeletingPortfolio_ThrowsException_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            portfolioRepository
                .Setup(r => r.Exists(portfolioId))
                .Returns(Task.FromResult(false));
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<PortfolioService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.DeletePortfolioAsync(portfolioId));
        }
    }
}