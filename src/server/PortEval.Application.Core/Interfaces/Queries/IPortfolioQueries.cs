using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IPortfolioQueries
    {
        Task<IEnumerable<PortfolioDto>> GetAllPortfoliosAsync();
        Task<PortfolioDto> GetPortfolioAsync(int portfolioId);
    }
}