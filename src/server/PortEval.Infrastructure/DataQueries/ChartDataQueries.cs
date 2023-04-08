using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Models.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.DataQueries;
using PortEval.Infrastructure.DataQueries.Models;

namespace PortEval.Infrastructure.DataQueries
{
    public class ChartDataQueries : IChartDataQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public ChartDataQueries(IDbConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
        }

        public async Task<IEnumerable<ChartDto>> GetChartsAsync()
        {
            // Chart queries have surrogate columns ToDateRangeSplit and NameSplit as a workaround to Dapper not splitting on NULL columns
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT Charts.Id,
  		                          Charts.Name,
  		                          Charts.Type,
		                          Frequency,
		                          Charts.CurrencyCode,
		                          DateRangeStart,
		                          DateRangeEnd,
		                          IsToDate,
                                  '' AS ToDateRangeSplit,
		                          ToDateRangeUnit,
                                  ToDateRangeValue,
		                          Width,
		                          Dash,
		                          Color,
		                          Lines.PortfolioId,
		                          Lines.PositionId,
		                          Lines.InstrumentId,
                                  '' AS NameSplit,
		                          PortfolioName,
		                          PositionName,
		                          InstrumentName
                          FROM dbo.Charts
                          LEFT JOIN (
	                          SELECT ChartId, Width, Dash, Color, PortfolioId, PositionId, InstrumentId FROM dbo.ChartLines
                          ) as Lines ON Lines.ChartId = Charts.Id
                          LEFT JOIN (
	                          SELECT Id, Name AS PortfolioName FROM dbo.Portfolios
                          ) as Portfolios ON Lines.PortfolioId = Portfolios.Id
                          LEFT JOIN (
	                          SELECT Positions.Id, Name AS PositionName FROM dbo.Positions LEFT JOIN dbo.Instruments ON Positions.InstrumentId = Instruments.Id
                          ) AS Positions ON Lines.PositionId = Positions.Id
                          LEFT JOIN (
	                          SELECT Id, Name AS InstrumentName FROM dbo.Instruments
                          ) AS Instruments ON Lines.InstrumentId = Instruments.Id
                          ORDER BY Charts.Name, PortfolioName, PositionName, InstrumentName";

            var charts = new Dictionary<int, ChartDto>();
            return await connection.QueryAsync<ChartDto, ToDateRangeQueryModel, ChartLineDto, ChartLineNameModel, ChartDto>(
                query,
                (chart, tdr, chartLine, lineNames) =>
                {
                    if (!charts.ContainsKey(chart.Id))
                    {
                        charts[chart.Id] = chart;
                        chart.Lines = new List<ChartLineDto>();
                    }

                    if (chart.IsToDate != null && (bool)chart.IsToDate)
                    {
                        charts[chart.Id].ToDateRange = new ToDateRange(tdr.ToDateRangeUnit, tdr.ToDateRangeValue);
                    }
                    if (chartLine != null)
                    {
                        charts[chart.Id].Lines.Add(AssignChartLineType(AssignChartLineName(chartLine, lineNames)));
                    }
                    return charts[chart.Id];
                },
                splitOn: "ToDateRangeSplit, Width, NameSplit");
        }

        public async Task<ChartDto> GetChartAsync(int chartId)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT Charts.Id,
  		                          Charts.Name,
  		                          Charts.Type,
		                          Frequency,
		                          Charts.CurrencyCode,
		                          DateRangeStart,
		                          DateRangeEnd,
		                          IsToDate,
                                  '' AS ToDateRangeSplit,
		                          ToDateRangeUnit,
                                  ToDateRangeValue,
		                          Width,
		                          Dash,
		                          Color,
		                          Lines.PortfolioId,
		                          Lines.PositionId,
		                          Lines.InstrumentId,
                                  '' AS NameSplit,
		                          PortfolioName,
		                          PositionName,
		                          InstrumentName
                          FROM dbo.Charts
                          LEFT JOIN (
	                          SELECT ChartId, Width, Dash, Color, PortfolioId, PositionId, InstrumentId FROM dbo.ChartLines
                          ) as Lines ON Lines.ChartId = Charts.Id
                          LEFT JOIN (
	                          SELECT Id, Name AS PortfolioName FROM dbo.Portfolios
                          ) as Portfolios ON Lines.PortfolioId = Portfolios.Id
                          LEFT JOIN (
	                          SELECT Positions.Id, Symbol AS PositionName FROM dbo.Positions LEFT JOIN dbo.Instruments ON Positions.InstrumentId = Instruments.Id
                          ) AS Positions ON Lines.PositionId = Positions.Id
                          LEFT JOIN (
	                          SELECT Id, Symbol AS InstrumentName FROM dbo.Instruments
                          ) AS Instruments ON Lines.InstrumentId = Instruments.Id
                          WHERE Charts.Id = @ChartId";

            ChartDto resultChart = null;

            await connection.QueryAsync<ChartDto, ToDateRangeQueryModel, ChartLineDto, ChartLineNameModel, ChartDto>(
                query,
                (chart, tdr, chartLine, lineNames) =>
                {
                    resultChart ??= chart;
                    chart.Lines = new List<ChartLineDto>();

                    chart.ToDateRange = new ToDateRange(tdr.ToDateRangeUnit, tdr.ToDateRangeValue);
                    if (chartLine != null)
                    {
                        resultChart.Lines.Add(AssignChartLineType(AssignChartLineName(chartLine, lineNames)));
                    }
                    return resultChart;
                },
                query,
                splitOn: "ToDateRangeSplit, Width, NameSplit");

            return resultChart;
        }

        private ChartLineDto AssignChartLineName(ChartLineDto chartLine, ChartLineNameModel lineNames)
        {
            if (lineNames.PortfolioName != null) chartLine.Name = lineNames.PortfolioName;
            else if (lineNames.PositionName != null) chartLine.Name = lineNames.PositionName;
            else if (lineNames.InstrumentName != null) chartLine.Name = lineNames.InstrumentName;

            return chartLine;
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
