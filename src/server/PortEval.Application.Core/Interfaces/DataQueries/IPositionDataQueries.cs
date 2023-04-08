using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.DataQueries
{
    public interface IPositionDataQueries
    {
        Task<IEnumerable<PositionDto>> GetAllPositionsAsync();
        Task<IEnumerable<PositionDto>> GetPortfolioPositionsAsync(int portfolioId);
        Task<PositionDto> GetPosition(int positionId);
    }
}