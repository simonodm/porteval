using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Infrastructure.Queries;

/// <inheritdoc cref="ICurrencyQueries" />
public class CurrencyQueries : ICurrencyQueries
{
    private readonly PortEvalDbConnectionCreator _connectionCreator;

    public CurrencyQueries(PortEvalDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync()
    {
        using var connection = _connectionCreator.CreateConnection();
        var query = @"SELECT Code, Name, Symbol, IsDefault FROM dbo.Currencies";

        return await connection.QueryAsync<CurrencyDto>(query);
    }

    /// <inheritdoc />
    public async Task<CurrencyDto> GetCurrencyAsync(string currencyCode)
    {
        using var connection = _connectionCreator.CreateConnection();
        var query = @"SELECT Code, Name, Symbol, IsDefault FROM dbo.Currencies
                          WHERE Code = @CurrencyCode";

        return await connection.QueryFirstOrDefaultAsync<CurrencyDto>(query, new { CurrencyCode = currencyCode });
    }

    /// <inheritdoc />
    public async Task<CurrencyDto> GetDefaultCurrencyAsync()
    {
        using var connection = _connectionCreator.CreateConnection();
        var query = @"SELECT Code, Name, Symbol, IsDefault FROM dbo.Currencies
                          WHERE IsDefault = 1";

        return await connection.QueryFirstOrDefaultAsync<CurrencyDto>(query);
    }

    /// <inheritdoc />
    public async Task<CurrencyExchangeRateDto> GetCurrencyExchangeRateAsync(string currencyFrom, string currencyTo,
        DateTime time)
    {
        using var connection = _connectionCreator.CreateConnection();
        var query = @"SELECT TOP 1 * FROM dbo.CurrencyExchangeRates
                         WHERE CurrencyFromCode = @BaseCurrencyCode
                         AND CurrencyToCode = @TargetCurrencyCode
                         AND Time <= @Time
                         ORDER BY Time DESC";

        return await connection.QueryFirstOrDefaultAsync<CurrencyExchangeRateDto>(query,
            new { BaseCurrencyCode = currencyFrom, TargetCurrencyCode = currencyTo, Time = time });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CurrencyExchangeRateDto>> GetDirectExchangeRatesAsync(string currencyCode,
        DateTime time)
    {
        using var connection = _connectionCreator.CreateConnection();
        var query = @"WITH row_num_added AS (
	                          SELECT Id, Time, ExchangeRate, CurrencyFromCode, CurrencyToCode, ROW_NUMBER() OVER (
		                          PARTITION BY CurrencyToCode
		                          ORDER BY Time DESC
	                          ) AS row_num
	                          FROM dbo.CurrencyExchangeRates
	                          WHERE Time <= @Time
                              AND CurrencyFromCode = @CurrencyCode
                          )
                          SELECT * FROM row_num_added
                          WHERE row_num = 1";

        return await connection.QueryAsync<CurrencyExchangeRateDto>(query,
            new { CurrencyCode = currencyCode, Time = time });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CurrencyExchangeRateDto>> GetDirectExchangeRatesAsync(string baseCurrencyCode,
        string targetCurrencyCode, DateRangeParams dateRange)
    {
        using var connection = _connectionCreator.CreateConnection();
        var query = @"SELECT * FROM dbo.CurrencyExchangeRates
                          WHERE CurrencyFromCode = @BaseCurrencyCode
                          AND CurrencyToCode = @TargetCurrencyCode
                          AND Time >= @TimeFrom
                          AND Time <= @TimeTo
                          ORDER BY Time";

        return await connection.QueryAsync<CurrencyExchangeRateDto>(query,
            new
            {
                BaseCurrencyCode = baseCurrencyCode,
                TargetCurrencyCode = targetCurrencyCode,
                TimeFrom = dateRange.From,
                TimeTo = dateRange.To
            });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CurrencyExchangeRateDto>> GetInversedExchangeRatesAsync(string baseCurrencyCode,
        string targetCurrencyCode, DateRangeParams dateRange)
    {
        using var connection = _connectionCreator.CreateConnection();
        var query =
            @"SELECT CurrencyToCode AS CurrencyFromCode, CurrencyFromCode AS CurrencyToCode, Time, 1/ExchangeRate AS ExchangeRate FROM dbo.CurrencyExchangeRates
                          WHERE CurrencyFromCode = @TargetCurrencyCode
                          AND CurrencyToCode = @BaseCurrencyCode
                          AND Time >= @TimeFrom
                          AND Time <= @TimeTo
                          ORDER BY Time";

        return await connection.QueryAsync<CurrencyExchangeRateDto>(query,
            new
            {
                BaseCurrencyCode = baseCurrencyCode,
                TargetCurrencyCode = targetCurrencyCode,
                TimeFrom = dateRange.From,
                TimeTo = dateRange.To
            });
    }
}