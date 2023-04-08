using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.DataQueries;

namespace PortEval.Infrastructure.DataQueries
{
    public class DashboardLayoutDataQueries : IDashboardLayoutDataQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public DashboardLayoutDataQueries(IDbConnectionCreator connectionCreator)
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
