using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Services;

namespace PortEval.Application.Core.Services
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
