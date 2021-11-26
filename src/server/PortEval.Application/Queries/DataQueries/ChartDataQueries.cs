using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Queries.DataQueries
{
    internal static class ChartDataQueries
    {
        public static QueryWrapper<IEnumerable<ChartDto>> GetCharts()
        {
            return new QueryWrapper<IEnumerable<ChartDto>>
            {
                Query = @"SELECT Id,
					 		      Name,
								  Type,
								  Frequency,
								  CurrencyCode,
								  DateRangeStart,
								  DateRangeEnd,
								  IsToDate,
								  ToDateRange,
								  Width,
								  Dash,
								  Color,
								  PortfolioId,
								  PositionId,
								  InstrumentId
						  FROM dbo.Charts
						  LEFT JOIN (
							  SELECT ChartId, Width, Dash, Color, PortfolioId, PositionId, InstrumentId FROM dbo.ChartLines
						  ) as Lines ON Lines.ChartId = Charts.Id
						  ORDER BY Charts.Name"
            };
        }

        public static QueryWrapper<ChartDto> GetChart(int chartId)
        {
            return new QueryWrapper<ChartDto>
            {
                Query = @"SELECT Id,
					 		      Name,
								  Type,
								  Frequency,
								  CurrencyCode,
								  DateRangeStart,
								  DateRangeEnd,
								  IsToDate,
								  ToDateRange,
								  Width,
								  Dash,
								  Color,
								  PortfolioId,
								  PositionId,
								  InstrumentId
						  FROM dbo.Charts
						  LEFT JOIN (
							  SELECT ChartId, Width, Dash, Color, PortfolioId, PositionId, InstrumentId FROM dbo.ChartLines
						  ) as Lines ON Lines.ChartId = Charts.Id
						  WHERE Id = @ChartId",
                Params = new { ChartId = chartId }
            };
        }
    }
}