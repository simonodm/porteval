using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Services;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services
{
    /// <inheritdoc cref="ICurrencyService" />
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ICurrencyDomainService _currencyDomainService;

        public CurrencyService(ICurrencyRepository currencyRepository, ICurrencyDomainService currencyDomainService)
        {
            _currencyRepository = currencyRepository;
            _currencyDomainService = currencyDomainService;
        }

        /// <inheritdoc cref="ICurrencyService.UpdateAsync" />
        public async Task UpdateAsync(CurrencyDto options)
        {
            var currencyEntity = await _currencyRepository.FindAsync(options.Code);
            if (currencyEntity == null)
            {
                throw new ItemNotFoundException($"Currency {options.Code} does not exist.");
            }

            var defaultCurrency = await _currencyRepository.GetDefaultCurrencyAsync();

            _currencyDomainService.ChangeDefaultCurrency(defaultCurrency, currencyEntity);
            defaultCurrency.IncreaseVersion();
            currencyEntity.IncreaseVersion();
            _currencyRepository.Update(defaultCurrency);
            _currencyRepository.Update(currencyEntity);
            await _currencyRepository.UnitOfWork.CommitAsync();
        }
    }
}
