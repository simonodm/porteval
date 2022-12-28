using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Repositories
{
    public interface IPositionRepository : IRepository
    {
        public Task<IEnumerable<Position>> ListPortfolioPositionsAsync(int portfolioId);
        public Task<IEnumerable<Position>> ListAllInstrumentPositionsAsync(int instrumentId);
        public Task<Position> FindAsync(int positionId);
        public Task<Position> FindParentPositionAsync(int transactionId);
        public Position Add(Position position);
        public Task DeleteAsync(int positionId);
        public void Delete(Position position);
        public Task<bool> ExistsAsync(int id);
        public Position Update(Position position);
    }
}