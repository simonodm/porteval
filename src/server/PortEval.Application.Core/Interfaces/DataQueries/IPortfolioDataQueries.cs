using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.DataQueries
{
    public interface IPortfolioDataQueries
    {
        Task<IEnumerable<PortfolioDto>> GetAllPortfoliosAsync();
        Task<PortfolioDto> GetPortfolio(int portfolioId);
    }
}