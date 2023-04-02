using System.Linq;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Queries.DataQueries;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Queries
{
    public class DashboardLayoutQueries : IDashboardLayoutQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public DashboardLayoutQueries(IDbConnectionCreator connection)
        {
            _connectionCreator = connection;
        }

        public async Task<QueryResponse<DashboardLayoutDto>> GetDashboardLayout()
        {
            var query = DashboardLayoutDataQueries.GetDashboardLayoutQuery();
            using var connection = _connectionCreator.CreateConnection();
            var dashboardItems = await connection.QueryAsync<DashboardItemDto>(query.Query, query.Params);

            var layout = new DashboardLayoutDto
            {
                Items = dashboardItems.ToList()
            };

            return new QueryResponse<DashboardLayoutDto>
            {
                Status = QueryStatus.Ok,
                Response = layout
            };
        }
    }
}
