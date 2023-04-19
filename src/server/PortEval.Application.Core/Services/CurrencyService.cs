using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Services;

namespace PortEval.Application.Core.Services;

/// <inheritdoc cref="ICurrencyService" />
public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyQueries _currencyDataQueries;
    private readonly ICurrencyDomainService _currencyDomainService;
    private readonly ICurrencyRepository _currencyRepository;

    /// <summary>
    ///     Initializes the service.
    /// </summary>
    public CurrencyService(ICurrencyRepository currencyRepository, ICurrencyDomainService currencyDomainService,
        ICurrencyQueries currencyDataQueries)
    {
        _currencyRepository = currencyRepository;
        _currencyDomainService = currencyDomainService;
        _currencyDataQueries = currencyDataQueries;
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<CurrencyDto>>> GetAllCurrenciesAsync()
    {
        var currencies = await _currencyDataQueries.GetAllCurrenciesAsync();
        return new OperationResponse<IEnumerable<CurrencyDto>>
        {
            Response = currencies
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<CurrencyDto>> GetCurrencyAsync(string currencyCode)
    {
        var currency = await _currencyDataQueries.GetCurrencyAsync(currencyCode);
        return new OperationResponse<CurrencyDto>
        {
            Response = currency,
            Status = currency != null ? OperationStatus.Ok : OperationStatus.NotFound
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<CurrencyDto>> UpdateAsync(CurrencyDto options)
    {
        var currencyEntity = await _currencyRepository.FindAsync(options.Code);
        if (currencyEntity == null)
            return new OperationResponse<CurrencyDto>
            {
                Status = OperationStatus.NotFound,
                Message = $"Currency {options.Code} does not exist."
            };

        var defaultCurrency = await _currencyRepository.GetDefaultCurrencyAsync();

        _currencyDomainService.ChangeDefaultCurrency(defaultCurrency, currencyEntity);
        defaultCurrency.IncreaseVersion();
        currencyEntity.IncreaseVersion();
        _currencyRepository.Update(defaultCurrency);
        _currencyRepository.Update(currencyEntity);
        await _currencyRepository.UnitOfWork.CommitAsync();

        return await GetCurrencyAsync(options.Code);
    }
}