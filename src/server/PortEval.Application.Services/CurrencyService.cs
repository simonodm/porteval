using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Services
{
    /// <inheritdoc cref="ICurrencyService" />
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        /// <inheritdoc cref="ICurrencyService.UpdateAsync" />
        public async Task UpdateAsync(CurrencyDto options)
        {
            var currencyEntity = await _currencyRepository.FindAsync(options.Code);
            if (currencyEntity == null)
            {
                throw new ItemNotFoundException($"Currency {options.Code} does not exist.");
            }

            if (options.IsDefault && !currencyEntity.IsDefault)
            {
                var currencies = await _currencyRepository.ListAllAsync();
                foreach (var currency in currencies.Where(c => c.IsDefault))
                {
                    currency.UnsetDefault();
                    _currencyRepository.Update(currency);
                }

                currencyEntity.SetAsDefault();
                currencyEntity.IncreaseVersion();
                _currencyRepository.Update(currencyEntity);

                await _currencyRepository.UnitOfWork.CommitAsync();
            }
        }
    }
}
