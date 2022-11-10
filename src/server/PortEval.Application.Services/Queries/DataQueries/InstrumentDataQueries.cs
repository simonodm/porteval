using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Queries.Models;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Services.Queries.DataQueries
{
    internal static class InstrumentDataQueries
    {
        public static QueryWrapper<IEnumerable<InstrumentDto>> GetAllInstrumentsQuery()
        {
            return new QueryWrapper<IEnumerable<InstrumentDto>>
            {
                Query =
                    @"SELECT dbo.Instruments.Id, Name, Symbol, Exchange, Type, CurrencyCode, Price as CurrentPrice, Note, IsTracked, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Instruments
                      LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY InstrumentId ORDER BY Time DESC) row_num, * FROM dbo.InstrumentPrices) AS p
                      ON p.InstrumentId = dbo.Instruments.Id
                      WHERE p.row_num = 1
                      OR p.row_num IS NULL
                      ORDER BY Symbol;"
            };
        }

        public static QueryWrapper<int> GetInstrumentCountQuery()
        {
            return new QueryWrapper<int>
            {
                Query = "SELECT COUNT(Id) FROM dbo.Instruments"
            };
        }

        public static QueryWrapper<IEnumerable<InstrumentDto>> GetInstrumentPageQuery(PaginationParams pagination)
        {
            return new QueryWrapper<IEnumerable<InstrumentDto>>
            {
                Query = @"SELECT dbo.Instruments.Id, Name, Symbol, Exchange, Type, CurrencyCode, Price as CurrentPrice, Note, IsTracked, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Instruments
                          LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY InstrumentId ORDER BY Time DESC) row_num, * FROM dbo.InstrumentPrices) AS p
                          ON p.InstrumentId = dbo.Instruments.Id
                          WHERE (p.row_num = 1
                          OR p.row_num IS NULL)
                          ORDER BY Symbol
                          OFFSET @Offset ROWS
                          FETCH NEXT @Rows ROWS ONLY",
                Params = new { Offset = (pagination.Page - 1) * pagination.Limit, Rows = pagination.Limit }
            };
        }

        public static QueryWrapper<InstrumentDto> GetInstrumentQuery(int instrumentId)
        {
            return new QueryWrapper<InstrumentDto>
            {
                Query = @"SELECT dbo.Instruments.Id, Name, Symbol, Exchange, Type, CurrencyCode, Price as CurrentPrice, Note, IsTracked, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Instruments
                          LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY InstrumentId ORDER BY Time DESC) row_num, * FROM dbo.InstrumentPrices) AS p
                          ON p.InstrumentId = dbo.Instruments.Id
                          WHERE (p.row_num = 1
                          OR p.row_num IS NULL)
                          AND dbo.Instruments.Id = @InstrumentId",
                Params = new { InstrumentId = instrumentId }
            };
        }

        public static QueryWrapper<IEnumerable<InstrumentDto>> GetInstrumentPrices(int instrumentId, DateTime from, DateTime to)
        {
            return new QueryWrapper<IEnumerable<InstrumentDto>>
            {
                Query = @"SELECT * FROM dbo.InstrumentPrices
                          WHERE InstrumentId = @InstrumentId
                          AND Time >= @TimeFrom
                          AND Time <= @TimeTo
                          ORDER BY Time DESC",
                Params = new { InstrumentId = instrumentId, TimeFrom = from, TimeTo = to }
            };
        }

        public static QueryWrapper<int> GetInstrumentPriceCount(int instrumentId, DateTime from, DateTime to, AggregationFrequency? frequency)
        {
            var partitionCalc = GetInstrumentPriceIntervalPartitionCalc(frequency);
            return new QueryWrapper<int>
            {
                Query = @$"WITH added_row_number AS (
	                          SELECT *, ROW_NUMBER() OVER(
                                  PARTITION BY {partitionCalc}
                                  ORDER BY [Time] DESC) AS row_number
                              FROM dbo.InstrumentPrices
                              WHERE InstrumentId = @InstrumentId
                              AND Time >= @TimeFrom
                              AND Time <= @TimeTo
                          )
                          SELECT COUNT(*) FROM added_row_number
                          WHERE row_number = 1",
                Params = new { InstrumentId = instrumentId, TimeFrom = from, TimeTo = to }
            };
        }

        public static QueryWrapper<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPage(int instrumentId, DateTime from, DateTime to,
            PaginationParams pagination, AggregationFrequency? frequency)
        {
            var partitionCalc = GetInstrumentPriceIntervalPartitionCalc(frequency);
            return new QueryWrapper<IEnumerable<InstrumentPriceDto>>
            {
                Query = @$"WITH added_row_number AS (
	                          SELECT *, ROW_NUMBER() OVER(
                                  PARTITION BY {partitionCalc}
                                  ORDER BY [Time] DESC) AS row_number
                              FROM dbo.InstrumentPrices
                              WHERE InstrumentId = @InstrumentId
                              AND Time >= @TimeFrom
                              AND Time <= @TimeTo
                          )
                          SELECT * FROM added_row_number
                          WHERE row_number = 1
                          ORDER BY Time DESC
                          OFFSET @Offset ROWS
                          FETCH NEXT @Rows ROWS ONLY",
                Params = new
                {
                    InstrumentId = instrumentId,
                    TimeFrom = from,
                    TimeTo = to,
                    Offset = (pagination.Page - 1) * pagination.Limit,
                    Rows = pagination.Limit
                }
            };
        }

        public static QueryWrapper<int> GetInstrumentPriceCompressedCount(int instrumentId, DateTime from, DateTime to,
            AggregationFrequency? frequency)
        {
            var partitionCalc = GetInstrumentPriceIntervalPartitionCalc(frequency);
            return new QueryWrapper<int>
            {
                Query = @$"WITH added_row_number_time AS (
	                          SELECT *,
  		                          ROW_NUMBER() OVER(
			                          PARTITION BY {partitionCalc}
			                          ORDER BY [Time] DESC) AS row_number_time
                              FROM dbo.InstrumentPrices
                              WHERE InstrumentId = @InstrumentId
                              AND Time >= @TimeFrom
                              AND Time <= @TimeTo
                          ),
                          aggregated_prices AS (
	                          SELECT *
	                          FROM added_row_number_time
	                          WHERE row_number_time = 1
                          ),
                          added_prev_price AS (
	                          SELECT *, LAG([Price], 1, 0) OVER (ORDER BY [Time]) AS prev_price
	                          FROM aggregated_prices
                          )
                          SELECT COUNT(*) FROM added_prev_price
                          WHERE [Price] <> prev_price",
                Params = new
                {
                    InstrumentId = instrumentId,
                    TimeFrom = from,
                    TimeTo = to
                }
            };
        }

        public static QueryWrapper<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPageCompressed(int instrumentId, DateTime from, DateTime to,
            PaginationParams pagination, AggregationFrequency? frequency)
        {
            var partitionCalc = GetInstrumentPriceIntervalPartitionCalc(frequency);
            return new QueryWrapper<IEnumerable<InstrumentPriceDto>>
            {
                Query = @$"WITH added_row_number_time AS (
	                          SELECT *,
  		                          ROW_NUMBER() OVER(
			                          PARTITION BY {partitionCalc}
			                          ORDER BY [Time] DESC) AS row_number_time
                              FROM dbo.InstrumentPrices
                              WHERE InstrumentId = @InstrumentId
                              AND Time >= @TimeFrom
                              AND Time <= @TimeTo
                          ),
                          aggregated_prices AS (
	                          SELECT *
	                          FROM added_row_number_time
	                          WHERE row_number_time = 1
                          ),
                          added_prev_price AS (
	                          SELECT *, LAG([Price], 1, 0) OVER (ORDER BY [Time]) AS prev_price
	                          FROM aggregated_prices
                          )
                          SELECT [Id], [Time], [Price], [InstrumentId] FROM added_prev_price
                          WHERE [Price] <> prev_price
                          ORDER BY Time DESC
                          OFFSET @Offset ROWS
                          FETCH NEXT @Rows ROWS ONLY",
                Params = new
                {
                    InstrumentId = instrumentId,
                    TimeFrom = from,
                    TimeTo = to,
                    Offset = (pagination.Page - 1) * pagination.Limit,
                    Rows = pagination.Limit
                }
            };
        }

        public static QueryWrapper<InstrumentPriceDto> GetInstrumentPrice(int instrumentId, DateTime time)
        {
            return new QueryWrapper<InstrumentPriceDto>
            {
                Query = @"SELECT TOP 1 * FROM dbo.InstrumentPrices
                          WHERE Time <= @Time
                          AND InstrumentId = @InstrumentId
                          ORDER BY Time DESC",
                Params = new { InstrumentId = instrumentId, Time = time }
            };
        }

        public static QueryWrapper<SingleTimeQueryModel> GetFirstPriceTime(int instrumentId)
        {
            return new QueryWrapper<SingleTimeQueryModel>
            {
                Query = @"SELECT MIN(Time) AS Time FROM dbo.InstrumentPrices
                          WHERE InstrumentId = @InstrumentId",
                Params = new { InstrumentId = instrumentId }
            };
        }

        private static string GetInstrumentPriceIntervalPartitionCalc(AggregationFrequency? frequency)
        {
            return frequency switch
            {
                null => "[Id]",
                AggregationFrequency.FiveMin => "DATEADD(minute, 5 * (DATEDIFF(minute, '', [Time]) / 5), '')",
                AggregationFrequency.Day => "DATEADD(day, DATEDIFF(day, '', [Time]), '')",
                AggregationFrequency.Hour => "DATEADD(hour, DATEDIFF(hour, '', [Time]), '')",
                AggregationFrequency.Week => "DATEADD(week, DATEDIFF(week, '', [Time]), '')",
                AggregationFrequency.Month => "DATEADD(month, DATEDIFF(month, '', [Time]), '')",
                AggregationFrequency.Year => "DATEADD(year, DATEDIFF(year, '', [Time]), '')",
                _ => throw new Exception($"Unknown aggregation frequency supplied: {frequency}.")
            };
        }
    }
}
