using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IChartQueries
    {
        Task<IEnumerable<ChartDto>> GetChartsAsync();
        Task<ChartDto> GetChartAsync(int chartId);
    }
}