using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Services.BulkImportExport
{
    public class PortfolioImportProcessor : ImportProcessor<PortfolioDto, PortfolioDtoValidator>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public PortfolioImportProcessor(IPortfolioRepository portfolioRepository, ICurrencyRepository currencyRepository) : base(portfolioRepository.UnitOfWork)
        {
            _portfolioRepository = portfolioRepository;
            _currencyRepository = currencyRepository;
        }

        public override async Task<ErrorLogEntry<PortfolioDto>> ProcessItem(PortfolioDto row)
        {
            var logEntry = new ErrorLogEntry<PortfolioDto>(row);
            if (!await _currencyRepository.Exists(row.CurrencyCode))
            {
                logEntry.AddError($"Unknown currency: {row.CurrencyCode}.");
                return logEntry;
            }

            var existingPortfolio = row.Id != default
                ? await _portfolioRepository.FindAsync(row.Id)
                : null;

            if (existingPortfolio == null)
            {
                _portfolioRepository.Add(new Portfolio(row.Name, row.Note,
                    row.CurrencyCode));
            }
            else
            {
                existingPortfolio.Rename(row.Name);
                existingPortfolio.SetNote(row.Note);
                existingPortfolio.ChangeCurrency(row.CurrencyCode);
                _portfolioRepository.Update(existingPortfolio);
            }

            return logEntry;
        }
    }
}
