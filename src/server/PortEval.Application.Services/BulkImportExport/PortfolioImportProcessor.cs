using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Application.Services.Interfaces;
using System.Threading.Tasks;

namespace PortEval.Application.Services.BulkImportExport
{
    public class PortfolioImportProcessor : ImportProcessor<PortfolioDto, PortfolioDtoValidator>
    {
        private readonly IPortfolioService _portfolioService;

        public PortfolioImportProcessor(IPortfolioService portfolioService) : base()
        {
            _portfolioService = portfolioService;
        }

        public override async Task<ProcessedRowErrorLogEntry<PortfolioDto>> ProcessItem(PortfolioDto row)
        {
            var logEntry = new ProcessedRowErrorLogEntry<PortfolioDto>(row);

            if (row.Id == default)
            {
                await _portfolioService.CreatePortfolioAsync(row);
            }
            else
            {
                var portfolio = await _portfolioService.UpdatePortfolioAsync(row);
                logEntry.Data.Id = portfolio.Id;
            }

            return logEntry;
        }
    }
}
