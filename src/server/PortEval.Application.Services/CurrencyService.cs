using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using PortEval.Application.Services.Interfaces.BackgroundJobs;

namespace PortEval.Application.Services
{
    /// <inheritdoc cref="ICurrencyService" />
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CurrencyService(ICurrencyRepository currencyRepository, IBackgroundJobClient backgroundJobClient)
        {
            _currencyRepository = currencyRepository;
            _backgroundJobClient = backgroundJobClient;
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
                var defaultCurrency = await _currencyRepository.GetDefaultCurrencyAsync();
                defaultCurrency.UnsetDefault();
                defaultCurrency.IncreaseVersion();
                _currencyRepository.Update(defaultCurrency);

                currencyEntity.SetAsDefault();
                currencyEntity.IncreaseVersion();
                _currencyRepository.Update(currencyEntity);

                await _currencyRepository.UnitOfWork.CommitAsync();
                _backgroundJobClient.Enqueue<IMissingExchangeRatesFetchJob>(job => job.Run());
            }
        }
    }
}
