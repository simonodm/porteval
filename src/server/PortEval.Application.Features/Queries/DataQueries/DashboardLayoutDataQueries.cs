using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Features.Queries.DataQueries
{
    internal static class DashboardLayoutDataQueries
    {
        public static QueryWrapper<DashboardItemDto> GetDashboardLayoutQuery()
        {
            return new QueryWrapper<DashboardItemDto>
            {
                Query = @"SELECT * FROM dbo.DashboardItems"
            };
        }
    }
}
