using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.DataQueries;

namespace PortEval.Infrastructure.DataQueries
{
    public class InstrumentDataQueries : IInstrumentDataQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;

        public InstrumentDataQueries(IDbConnectionCreator connectionCreator)
        {
            _connectionCreator = connectionCreator;
        }

        public async Task<IEnumerable<InstrumentDto>> GetAllInstrumentsAsync()
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT dbo.Instruments.Id, Name, Symbol, Exchange, Type, CurrencyCode, Price as CurrentPrice, Note, TrackingStatus, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Instruments
                        LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY InstrumentId ORDER BY Time DESC) row_num, * FROM dbo.InstrumentPrices) AS p
                        ON p.InstrumentId = dbo.Instruments.Id
                        WHERE p.row_num = 1
                        OR p.row_num IS NULL
                        ORDER BY Name";

            return await connection.QueryAsync<InstrumentDto>(query);
        }

        public async Task<int> GetInstrumentCountAsync()
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = "SELECT COUNT(Id) FROM dbo.Instruments";

            return await connection.QueryFirstAsync<int>(query);
        }

        public async Task<IEnumerable<InstrumentDto>> GetInstrumentPageAsync(PaginationParams pagination)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT dbo.Instruments.Id, Name, Symbol, Exchange, Type, CurrencyCode, Price as CurrentPrice, Note, TrackingStatus, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Instruments
                          LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY InstrumentId ORDER BY Time DESC) row_num, * FROM dbo.InstrumentPrices) AS p
                          ON p.InstrumentId = dbo.Instruments.Id
                          WHERE (p.row_num = 1
                          OR p.row_num IS NULL)
                          ORDER BY Name
                          OFFSET @Offset ROWS
                          FETCH NEXT @Rows ROWS ONLY";

            return await connection.QueryAsync<InstrumentDto>(query,
                new { Offset = (pagination.Page - 1) * pagination.Limit, Rows = pagination.Limit });
        }

        public async Task<InstrumentDto> GetInstrumentAsync(int instrumentId)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT dbo.Instruments.Id, Name, Symbol, Exchange, Type, CurrencyCode, Price as CurrentPrice, Note, TrackingStatus, TrackingInfo_LastUpdate as LastPriceUpdate FROM dbo.Instruments
                          LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY InstrumentId ORDER BY Time DESC) row_num, * FROM dbo.InstrumentPrices) AS p
                          ON p.InstrumentId = dbo.Instruments.Id
                          WHERE (p.row_num = 1
                          OR p.row_num IS NULL)
                          AND dbo.Instruments.Id = @InstrumentId";

            return await connection.QueryFirstOrDefaultAsync<InstrumentDto>(query, new { InstrumentId = instrumentId });
        }

        public async Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesAsync(int instrumentId, DateTime from,
            DateTime to)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT * FROM dbo.InstrumentPrices
                          WHERE InstrumentId = @InstrumentId
                          AND Time >= @TimeFrom
                          AND Time <= @TimeTo
                          ORDER BY Time";

            return await connection.QueryAsync<InstrumentPriceDto>(query);
        }

        public async Task<int> GetInstrumentPriceCountAsync(int instrumentId, DateTime from, DateTime to,
            AggregationFrequency? frequency)
        {
            using var connection = _connectionCreator.CreateConnection();
            var partitionCalc = GetInstrumentPriceIntervalPartitionCalc(frequency);
            var query = @$"WITH added_row_number AS (
	                          SELECT *, ROW_NUMBER() OVER(
                                  PARTITION BY {partitionCalc}
                                  ORDER BY [Time] DESC) AS row_number
                              FROM dbo.InstrumentPrices
                              WHERE InstrumentId = @InstrumentId
                              AND Time >= @TimeFrom
                              AND Time <= @TimeTo
                          )
                          SELECT COUNT(*) FROM added_row_number
                          WHERE row_number = 1";

            return await connection.QueryFirstAsync<int>(query,
                new { InstrumentId = instrumentId, TimeFrom = from, TimeTo = to });
        }

        public async Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPage(
            int instrumentId,
            DateTime from,
            DateTime to,
            PaginationParams pagination,
            AggregationFrequency? frequency)
        {
            using var connection = _connectionCreator.CreateConnection();
            var partitionCalc = GetInstrumentPriceIntervalPartitionCalc(frequency);
            var query = @$"WITH added_row_number AS (
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
                          FETCH NEXT @Rows ROWS ONLY";

            return await connection.QueryAsync<InstrumentPriceDto>(query, new
            {
                InstrumentId = instrumentId,
                TimeFrom = from,
                TimeTo = to,
                Offset = (pagination.Page - 1) * pagination.Limit,
                Rows = pagination.Limit
            });
        }

        public async Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPriceCompressedCount(int instrumentId,
            DateTime from, DateTime to,
            AggregationFrequency? frequency)
        {
            using var connection = _connectionCreator.CreateConnection();
            var partitionCalc = GetInstrumentPriceIntervalPartitionCalc(frequency);
            var query = @$"WITH added_row_number_time AS (
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
                          WHERE [Price] <> prev_price";

            return await connection.QueryAsync<InstrumentPriceDto>(query, new
            {
                InstrumentId = instrumentId,
                TimeFrom = from,
                TimeTo = to
            });
        }

        public async Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPageCompressed(int instrumentId,
            DateTime from, DateTime to,
            PaginationParams pagination, AggregationFrequency? frequency)
        {
            using var connection = _connectionCreator.CreateConnection();
            var partitionCalc = GetInstrumentPriceIntervalPartitionCalc(frequency);
            var query = @$"WITH added_row_number_time AS (
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
                          FETCH NEXT @Rows ROWS ONLY";

            return await connection.QueryAsync<InstrumentPriceDto>(query, new
            {
                InstrumentId = instrumentId,
                TimeFrom = from,
                TimeTo = to,
                Offset = (pagination.Page - 1) * pagination.Limit,
                Rows = pagination.Limit
            });
        }

        public async Task<InstrumentPriceDto> GetInstrumentPriceAsync(int instrumentId, DateTime time)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query = @"SELECT TOP 1 * FROM dbo.InstrumentPrices
                          WHERE Time <= @Time
                          AND InstrumentId = @InstrumentId
                          ORDER BY Time DESC";

            return await connection.QueryFirstOrDefaultAsync<InstrumentPriceDto>(query,
                new { InstrumentId = instrumentId, Time = time });
        }

        public async Task<IEnumerable<InstrumentSplitDto>> GetInstrumentSplitsAsync(int instrumentId)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query =
                @"SELECT Id, InstrumentId, Time, SplitRatioDenominator, SplitRatioNumerator, ProcessingStatus AS Status FROM [dbo].[InstrumentSplits]
                  WHERE InstrumentId = @InstrumentId";

            return await connection.QueryAsync<InstrumentSplitDto>(query, new { InstrumentId = instrumentId });
        }

        public async Task<InstrumentSplitDto> GetInstrumentSplitAsync(int instrumentId, int splitId)
        {
            using var connection = _connectionCreator.CreateConnection();
            var query =
                @"SELECT Id, InstrumentId, Time, SplitRatioDenominator, SplitRatioNumerator, ProcessingStatus AS Status FROM [dbo].[InstrumentSplits]
                  WHERE InstrumentId = @InstrumentId AND Id = @SplitId";

            return await connection.QueryFirstOrDefaultAsync<InstrumentSplitDto>(query, new { InstrumentId = instrumentId, SplitId = splitId });
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
