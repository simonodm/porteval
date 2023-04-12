using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Queries
{
    public class DashboardLayoutQueries : IDashboardLayoutQueries
    {
        private readonly PortEvalDbConnectionCreator _connectionCreator;

        public DashboardLayoutQueries(PortEvalDbConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
        }

        public async Task<IEnumerable<DashboardItemDto>> GetDashboardItemsAsync()
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT * FROM dbo.DashboardItems";

            return await connection.QueryAsync<DashboardItemDto>(query);
        }
    }
}
