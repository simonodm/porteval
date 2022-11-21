using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    /// <inheritdoc cref="IPortfolioRepository"/>
    public class PortfolioRepository : IPortfolioRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public PortfolioRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="IPortfolioRepository.ListAllAsync"/>
        public async Task<IEnumerable<Portfolio>> ListAllAsync()
        {
            return await _context.Portfolios.OrderBy(p => p.Name).ToListAsync();
        }

        /// <inheritdoc cref="IPortfolioRepository.FindAsync"/>
        public async Task<Portfolio> FindAsync(int id)
        {
            var portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == id);

            return portfolio;
        }

        /// <inheritdoc cref="IPortfolioRepository.Add"/>
        public Portfolio Add(Portfolio portfolio)
        {
            var createdPortfolio = _context.Portfolios.Add(portfolio).Entity;
            return createdPortfolio;
        }

        /// <inheritdoc cref="IPortfolioRepository.Update"/>
        public Portfolio Update(Portfolio portfolio)
        {
            var updatedPortfolio = _context.Portfolios.Update(portfolio).Entity;
            return updatedPortfolio;
        }

        /// <inheritdoc cref="IPortfolioRepository.DeleteAsync"/>
        public async Task DeleteAsync(int portfolioId)
        {
            var foundPortfolioEntity = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == portfolioId);
            if(foundPortfolioEntity != null)
            {
                var foundPortfolioLines = _context.ChartLines
                    .Where(line => EF.Property<string>(line, "Line_Type") == "Portfolio" && (line as ChartLinePortfolio).PortfolioId == portfolioId);
                var foundPositionLines = _context.ChartLines
                    .Where(line => EF.Property<string>(line, "Line_Type") == "Position")
                    .Join(_context.Positions, line => (line as ChartLinePosition).PositionId, position => position.Id, (line, position) => line);

                _context.ChartLines.RemoveRange(foundPortfolioLines);
                _context.ChartLines.RemoveRange(foundPositionLines);
                _context.Portfolios.Remove(foundPortfolioEntity);
            }
        }

        /// <inheritdoc cref="IPortfolioRepository.ExistsAsync"/>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Portfolios.AnyAsync(i => i.Id == id);
        }
    }
}
