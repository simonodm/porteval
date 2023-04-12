using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IPositionQueries
    {
        Task<IEnumerable<PositionDto>> GetAllPositionsAsync();
        Task<IEnumerable<PositionDto>> GetPortfolioPositionsAsync(int portfolioId);
        Task<PositionDto> GetPositionAsync(int positionId);
    }
}