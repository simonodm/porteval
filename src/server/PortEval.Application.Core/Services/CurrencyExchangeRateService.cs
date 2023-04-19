using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Services;

/// <inheritdoc cref="ICurrencyExchangeRateService" />
public class CurrencyExchangeRateService : ICurrencyExchangeRateService
{
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICurrencyQueries _currencyDataQueries;

    /// <summary>
    ///     Initializes the service.
    /// </summary>
    public CurrencyExchangeRateService(ICurrencyQueries currencyDataQueries, ICurrencyConverter currencyConverter)
    {
        _currencyDataQueries = currencyDataQueries;
        _currencyConverter = currencyConverter;
    }

    /// <inheritdoc />
    public async Task<OperationResponse<CurrencyExchangeRateDto>> GetExchangeRateAtAsync(string currencyFrom,
        string currencyTo, DateTime
            time)
    {
        var baseCurrency = await _currencyDataQueries.GetCurrencyAsync(currencyFrom);
        if (baseCurrency == null)
            return new OperationResponse<CurrencyExchangeRateDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Currency {currencyFrom} does not exist."
            };

        var targetCurrency = await _currencyDataQueries.GetCurrencyAsync(currencyTo);
        if (targetCurrency == null)
            return new OperationResponse<CurrencyExchangeRateDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Currency {currencyTo} does not exist."
            };

        var exchangeRate = await _currencyDataQueries.GetCurrencyExchangeRateAsync(currencyFrom, currencyTo, time);
        return new OperationResponse<CurrencyExchangeRateDto>
        {
            Status = exchangeRate != null ? OperationStatus.Ok : OperationStatus.NotFound,
            Message = exchangeRate != null
                ? ""
                : $"No exchange rate found from {currencyFrom} to {currencyTo} at {time}."
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRatesAsync(
        string currencyCode,
        DateTime time)
    {
        var currency = await _currencyDataQueries.GetCurrencyAsync(currencyCode);
        if (currency == null)
            return new OperationResponse<IEnumerable<CurrencyExchangeRateDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Currency {currencyCode} does not exist."
            };

        var exchangeRates = await _currencyDataQueries.GetDirectExchangeRatesAsync(currencyCode, time);

        return new OperationResponse<IEnumerable<CurrencyExchangeRateDto>>
        {
            Status = OperationStatus.Ok,
            Response = exchangeRates
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRatesAsync(
        string currencyFrom,
        string currencyTo, DateRangeParams dateRange)
    {
        var baseCurrency = await _currencyDataQueries.GetCurrencyAsync(currencyFrom);
        if (baseCurrency == null)
            return new OperationResponse<IEnumerable<CurrencyExchangeRateDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Currency {currencyFrom} does not exist."
            };

        var targetCurrency = await _currencyDataQueries.GetCurrencyAsync(currencyTo);
        if (targetCurrency == null)
            return new OperationResponse<IEnumerable<CurrencyExchangeRateDto>>
            {
                Status = OperationStatus.NotFound,
                Message = $"Currency {currencyTo} does not exist."
            };

        IEnumerable<CurrencyExchangeRateDto> result;

        if (baseCurrency.IsDefault)
        {
            result = await _currencyDataQueries.GetDirectExchangeRatesAsync(currencyFrom, currencyTo, dateRange);
        }
        else if (targetCurrency.IsDefault)
        {
            result = await _currencyDataQueries.GetInversedExchangeRatesAsync(currencyFrom, currencyTo, dateRange);
        }
        else
        {
            var defaultCurrency = await _currencyDataQueries.GetDefaultCurrencyAsync();
            if (defaultCurrency == null)
                return new OperationResponse<IEnumerable<CurrencyExchangeRateDto>>
                {
                    Status = OperationStatus.Error,
                    Message = "No default currency is set."
                };

            var baseToDefaultExchangeRates =
                await _currencyDataQueries.GetInversedExchangeRatesAsync(currencyFrom, defaultCurrency.Code,
                    dateRange);
            var defaultToTargetExchangeRates =
                await _currencyDataQueries.GetDirectExchangeRatesAsync(defaultCurrency.Code, currencyTo, dateRange);

            result = _currencyConverter.CombineExchangeRates(baseToDefaultExchangeRates,
                defaultToTargetExchangeRates);
        }

        return new OperationResponse<IEnumerable<CurrencyExchangeRateDto>>
        {
            Status = OperationStatus.Ok,
            Response = result
        };
    }
}