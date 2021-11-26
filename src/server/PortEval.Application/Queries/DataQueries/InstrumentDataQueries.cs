using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Queries.Models;
using System;
using System.Collections.Generic;

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
                Query = @"SELECT Id, Name, Symbol, Exchange, Type, CurrencyCode FROM dbo.Instruments
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
                Query = @"SELECT Id, Name, Symbol, Exchange, Type, CurrencyCode FROM dbo.Instruments
                          WHERE Id = @InstrumentId",
                Params = new { InstrumentId = instrumentId }
            };
        }

        public static QueryWrapper<int> GetInstrumentPriceCount(int instrumentId, DateTime from, DateTime to)
        {
            return new QueryWrapper<int>
            {
                Query = @"SELECT COUNT(Id) FROM dbo.InstrumentPrices
                          WHERE InstrumentId = @InstrumentId
                          AND Time >= @TimeFrom
                          AND Time <= @TimeTo",
                Params = new { InstrumentId = instrumentId, TimeFrom = from, TimeTo = to }
            };
        }

        public static QueryWrapper<IEnumerable<InstrumentPriceDto>> GetInstrumentPrices(int instrumentId, DateTime from, DateTime to, PaginationParams pagination)
        {
            return new QueryWrapper<IEnumerable<InstrumentPriceDto>>
            {
                Query = @"SELECT * FROM dbo.InstrumentPrices
                          WHERE InstrumentId = @InstrumentId
                          AND Time >= @TimeFrom
                          AND Time <= @TimeTo
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
    }
}
