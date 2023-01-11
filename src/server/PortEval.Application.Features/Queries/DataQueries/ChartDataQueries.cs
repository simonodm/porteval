using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Features.Queries.DataQueries
{
    internal static class ChartDataQueries
    {
        // Chart queries have surrogate columns called ToDateRangeSplit and NameSplit as a workaround to Dapper not splitting on NULL columns.

        public static QueryWrapper<IEnumerable<ChartDto>> GetCharts()
        {
            return new QueryWrapper<IEnumerable<ChartDto>>
            {
                Query = @"SELECT Charts.Id,
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
                          ORDER BY Charts.Name, PortfolioName, PositionName, InstrumentName"
            };
        }

        public static QueryWrapper<ChartDto> GetChart(int chartId)
        {
            return new QueryWrapper<ChartDto>
            {
                Query = @"SELECT Charts.Id,
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
                          WHERE Charts.Id = @ChartId",
                Params = new { ChartId = chartId }
            };
        }
    }
}