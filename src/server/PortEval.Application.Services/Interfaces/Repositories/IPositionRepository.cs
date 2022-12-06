using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    public interface IPositionRepository : IRepository
    {
        public Position Add(Position position);
        public Task DeleteAsync(int positionId);
        public void Delete(Position position);
        public Task<bool> ExistsAsync(int id);
        public Task<Position> FindAsync(int positionId);
        public Task<Position> FindParentPositionAsync(int transactionId);
        public Task<IEnumerable<Position>> ListPortfolioPositionsAsync(int portfolioId);
        public Position Update(Position position);
    }
}