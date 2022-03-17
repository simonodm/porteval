using System.Data;

namespace PortEval.Application.Services.Queries.Interfaces
{
    public interface IDbConnectionCreator
    {
        public IDbConnection CreateConnection();
    }
}
