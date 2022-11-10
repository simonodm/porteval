﻿using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Services.Queries.DataQueries
{
    internal static class ExchangeDataQueries
    {
        public static QueryWrapper<ExchangeDto> GetKnownExchangesQuery()
        {
            return new QueryWrapper<ExchangeDto>
            {
                Query = @"SELECT * FROM [dbo].[Exchanges] ORDER BY [Symbol]"
            };
        }
    }
}
