using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.DataQueries
{
    public interface IChartDataQueries
    {
        Task<IEnumerable<ChartDto>> GetChartsAsync();
        Task<ChartDto> GetChartAsync(int chartId);
    }
}