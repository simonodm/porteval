using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure.Repositories;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class PortfolioRepositoryTests : RepositoryTestBase
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioRepositoryTests() : base()
        {
            _portfolioRepository = new PortfolioRepository(DbContext);

            DbContext.Add(new Currency("USD", "US Dollar", "$", true));
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task ListAllAsync_ReturnsAllPortfolios()
        {
            var first = new Portfolio("Test1", "", "USD");
            var second = new Portfolio("Test2", "TestNote", "USD");

            DbContext.Add(first);
            DbContext.Add(second);
            await DbContext.SaveChangesAsync();

            var portfolios = await _portfolioRepository.ListAllAsync();

            Assert.Collection(portfolios, portfolio => AssertPortfoliosAreEqual(first, portfolio),
                portfolio => AssertPortfoliosAreEqual(second, portfolio));
        }

        [Fact]
        public async Task FindAsync_ReturnsCorrectPortfolio()
        {
            var portfolio = new Portfolio("Portfolio", "TestNote", "USD");
            DbContext.Add(portfolio);
            await DbContext.SaveChangesAsync();

            var foundPortfolio = await _portfolioRepository.FindAsync(portfolio.Id);

            AssertPortfoliosAreEqual(portfolio, foundPortfolio);
        }

        [Fact]
        public async Task Add_CreatesNewPortfolio()
        {
            var portfolio = new Portfolio("Portfolio", "TestNote", "USD");

            _portfolioRepository.Add(portfolio);
            await _portfolioRepository.UnitOfWork.CommitAsync();

            var createdPortfolio = DbContext.Portfolios.FirstOrDefault();

            AssertPortfoliosAreEqual(portfolio, createdPortfolio);
        }

        [Fact]
        public async Task Update_UpdatesPortfolio()
        {
            var portfolio = new Portfolio("Portfolio", "TestNote", "USD");
            DbContext.Add(portfolio);
            await DbContext.SaveChangesAsync();

            portfolio.Rename("TEST");
            portfolio.SetNote("TEST");

            _portfolioRepository.Update(portfolio);
            await _portfolioRepository.UnitOfWork.CommitAsync();

            var updatedPortfolio = DbContext.Portfolios.FirstOrDefault();

            AssertPortfoliosAreEqual(portfolio, updatedPortfolio);
        }

        [Fact]
        public async Task DeleteAsync_DeletesPortfolio()
        {
            var portfolio = new Portfolio("Portfolio", "TestNote", "USD");
            DbContext.Add(portfolio);
            await DbContext.SaveChangesAsync();

            await _portfolioRepository.DeleteAsync(portfolio.Id);
            await _portfolioRepository.UnitOfWork.CommitAsync();

            var portfolioDeleted = !DbContext.Portfolios.Any();

            Assert.True(portfolioDeleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenPortfolioExists()
        {
            var portfolio = new Portfolio("Portfolio", "TestNote", "USD");
            DbContext.Portfolios.Add(portfolio);
            await DbContext.SaveChangesAsync();

            var exists = await _portfolioRepository.ExistsAsync(portfolio.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenPortfolioDoesNotExist()
        {
            var exists = await _portfolioRepository.ExistsAsync(1);

            Assert.False(exists);
        }

        private void AssertPortfoliosAreEqual(Portfolio expected, Portfolio actual)
        {
            Assert.Equal(expected?.Name, actual?.Name);
            Assert.Equal(expected?.CurrencyCode, actual?.CurrencyCode);
            Assert.Equal(expected?.Note, actual?.Note);
        }
    }
}
