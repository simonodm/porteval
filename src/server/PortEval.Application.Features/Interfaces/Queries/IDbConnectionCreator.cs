using System.Data;

namespace PortEval.Application.Features.Interfaces.Queries
{
    public interface IDbConnectionCreator
    {
        public IDbConnection CreateConnection();
    }
}
