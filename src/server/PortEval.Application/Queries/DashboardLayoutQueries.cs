using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Queries.DataQueries;
using PortEval.Application.Queries.Interfaces;
using PortEval.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Queries
{
    public class DashboardLayoutQueries : IDashboardLayoutQueries
    {
        private readonly PortEvalDbConnection _connection;

        public DashboardLayoutQueries(PortEvalDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<QueryResponse<DashboardLayoutDto>> GetDashboardLayout()
        {
            var query = DashboardLayoutDataQueries.GetDashboardLayoutQuery();
            using var connection = _connection.CreateConnection();
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
