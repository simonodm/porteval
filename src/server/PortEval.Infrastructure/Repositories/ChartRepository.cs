using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    /// <inheritdoc cref="IChartRepository"/>
    public class ChartRepository : IChartRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public ChartRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="IChartRepository.ListAllAsync"/>
        public async Task<IEnumerable<Chart>> ListAllAsync()
        {
            return await _context.Charts.Include(c => c.Lines).OrderBy(c => c.Name).ToListAsync();
        }

        /// <inheritdoc cref="IChartRepository.FindAsync"/>
        public async Task<Chart> FindAsync(int id)
        {
            var chart = await _context.Charts
                .Include(c => c.Lines)
                .FirstOrDefaultAsync(c => c.Id == id);

            return chart;
        }

        /// <inheritdoc cref="IChartRepository.Add"/>
        public Chart Add(Chart chart)
        {
            var newChart = _context.Charts.Add(chart).Entity;
            return newChart;
        }

        /// <inheritdoc cref="IChartRepository.Update"/>
        public Chart Update(Chart chart)
        {
            var updatedChart = _context.Charts.Update(chart).Entity;
            return updatedChart;
        }

        /// <inheritdoc cref="IChartRepository.DeleteAsync"/>
        public async Task DeleteAsync(int chartId)
        {
            var foundChartEntity = await _context.Charts.FirstOrDefaultAsync(c => c.Id == chartId);
            if (foundChartEntity != null)
            {
                Delete(foundChartEntity);
            }
        }

        /// <inheritdoc cref="IChartRepository.Delete" />
        public void Delete(Chart chart)
        {
            _context.Charts.Remove(chart);
        }

        /// <inheritdoc cref="IChartRepository.ExistsAsync"/>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Charts.AnyAsync(c => c.Id == id);
        }
    }
}
