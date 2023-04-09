using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IChartQueries
    {
        Task<IEnumerable<ChartDto>> GetChartsAsync();
        Task<ChartDto> GetChartAsync(int chartId);
    }
}