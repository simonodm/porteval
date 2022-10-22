using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    public interface IPositionRepository : IRepository
    {
        public Position Add(Position position);
        public Task Delete(int positionId);
        public Task<bool> Exists(int id);
        public Task<Position> FindAsync(int positionId);
        public Task<Position> FindParentPosition(int transactionId);
        public Task<IEnumerable<Position>> ListAsync(int portfolioId);
        public Position Update(Position position);
    }
}