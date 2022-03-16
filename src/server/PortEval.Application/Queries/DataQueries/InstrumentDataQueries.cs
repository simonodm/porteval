using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Queries.Models;
using System;
using System.Collections.Generic;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Queries.DataQueries
{
    internal static class InstrumentDataQueries
    {
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
                Query = @"SELECT Id, Name, Symbol, Exchange, Type, CurrencyCode, Note, IsTracked, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Instruments
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
                Query = @"SELECT Id, Name, Symbol, Exchange, Type, CurrencyCode, Note, IsTracked, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Instruments
                          WHERE Id = @InstrumentId",
                Params = new { InstrumentId = instrumentId }
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
                                  ORDER BY [Time]) AS row_number
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

        public static QueryWrapper<IEnumerable<InstrumentExchangeQueryModel>> GetKnownExchangesQuery()
        {
            return new QueryWrapper<IEnumerable<InstrumentExchangeQueryModel>>
            {
                Query = @"SELECT DISTINCT Exchange FROM dbo.Instruments"
            };
        }

        public static QueryWrapper<IEnumerable<InstrumentPriceDto>> GetInstrumentPrices(int instrumentId, DateTime from, DateTime to,
            PaginationParams pagination, AggregationFrequency? frequency)
        {
            var partitionCalc = GetInstrumentPriceIntervalPartitionCalc(frequency);
            return new QueryWrapper<IEnumerable<InstrumentPriceDto>>
            {
                Query = @$"WITH added_row_number AS (
	                          SELECT *, ROW_NUMBER() OVER(
                                  PARTITION BY {partitionCalc}
                                  ORDER BY [Time]) AS row_number
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
                null => "[Time]",
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
