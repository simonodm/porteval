using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services
{
    /// <inheritdoc cref="IPortfolioService"/>
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IChartRepository _chartRepository;

        public PortfolioService(IPortfolioRepository portfolioRepository, ICurrencyRepository currencyRepository, IChartRepository chartRepository)
        {
            _portfolioRepository = portfolioRepository;
            _currencyRepository = currencyRepository;
            _chartRepository = chartRepository;
        }

        /// <inheritdoc cref="IPortfolioService.CreatePortfolioAsync"/>
        public async Task<Portfolio> CreatePortfolioAsync(PortfolioDto options)
        {
            if (!(await _currencyRepository.Exists(options.CurrencyCode)))
            {
                throw new ItemNotFoundException($"Currency {options.CurrencyCode} does not exist.");
            }

            var portfolio = _portfolioRepository.Add(new Portfolio(options.Name, options.Note, options.CurrencyCode));
            await _portfolioRepository.UnitOfWork.CommitAsync();
            return portfolio;
        }

        /// <inheritdoc cref="IPortfolioService.UpdatePortfolioAsync"/>
        public async Task<Portfolio> UpdatePortfolioAsync(PortfolioDto options)
        {
            var portfolio = await _portfolioRepository.FindAsync(options.Id);
            if (portfolio == null)
            {
                throw new ItemNotFoundException($"Update failed: portfolio {options.Id} does not exist.");
            }

            if (!(await _currencyRepository.Exists(options.CurrencyCode)))
            {
                throw new ItemNotFoundException($"Currency {options.CurrencyCode} does not exist.");
            }

            portfolio.Rename(options.Name);
            portfolio.SetNote(options.Note);
            portfolio.ChangeCurrency(options.CurrencyCode);
            portfolio.IncreaseVersion();
            _portfolioRepository.Update(portfolio);

            await _portfolioRepository.UnitOfWork.CommitAsync();
            return portfolio;
        }

        /// <inheritdoc cref="IPortfolioService.DeletePortfolioAsync"/>
        public async Task DeletePortfolioAsync(int id)
        {
            if (!(await _portfolioRepository.Exists(id)))
            {
                throw new ItemNotFoundException($"Delete failed: portfolio {id} does not exist.");
            }

            await _portfolioRepository.Delete(id);
            await _portfolioRepository.UnitOfWork.CommitAsync();
        }
    }
}
