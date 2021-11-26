using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Queries.DataQueries;
using PortEval.Application.Queries.Interfaces;
using PortEval.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Queries
{
    /// <inheritdoc cref="IChartQueries"/>
    public class ChartQueries : IChartQueries
    {
        private readonly PortEvalDbConnection _connection;

        public ChartQueries(PortEvalDbConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc cref="IChartQueries.GetCharts"/>
        public async Task<QueryResponse<IEnumerable<ChartDto>>> GetCharts()
        {
            var query = ChartDataQueries.GetCharts();

            var charts = new Dictionary<int, ChartDto>();

            using var connection = _connection.CreateConnection();
            await connection.QueryAsync<ChartDto, ChartLineDto, ChartDto>(
                query.Query,
                (chart, chartLine) =>
                {
                    if (!charts.ContainsKey(chart.Id))
                    {
                        charts[chart.Id] = chart;
                        chart.Lines = new List<ChartLineDto>();
                    }
                    if (chartLine != null)
                    {
                        charts[chart.Id].Lines.Add(AssignChartLineType(chartLine));
                    }
                    return charts[chart.Id];
                },
                query.Params,
                splitOn: "Width");

            return new QueryResponse<IEnumerable<ChartDto>>
            {
                Status = QueryStatus.Ok,
                Response = charts.Values
            };
        }

        /// <inheritdoc cref="IChartQueries.GetChart"/>
        public async Task<QueryResponse<ChartDto>> GetChart(int chartId)
        {
            var query = ChartDataQueries.GetChart(chartId);

            ChartDto resultChart = null;

            using var connection = _connection.CreateConnection();
            await connection.QueryAsync<ChartDto, ChartLineDto, ChartDto>(
                query.Query,
                (chart, chartLine) =>
                {
                    resultChart ??= chart;
                    chart.Lines = new List<ChartLineDto>();
                    if (chartLine != null)
                    {
                        resultChart.Lines.Add(AssignChartLineType(chartLine));
                    }
                    return resultChart;
                },
                query.Params,
                splitOn: "Width");

            return new QueryResponse<ChartDto>
            {
                Status = resultChart != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = resultChart
            };
        }

        private ChartLineDto AssignChartLineType(ChartLineDto chartLine)
        {
            if (chartLine.InstrumentId != null) chartLine.Type = ChartLineType.Instrument;
            else if (chartLine.PositionId != null) chartLine.Type = ChartLineType.Position;
            else if (chartLine.PortfolioId != null) chartLine.Type = ChartLineType.Portfolio;

            return chartLine;
        }
    }
}
