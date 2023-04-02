using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Services
{
    /// <inheritdoc />
    public class CurrencyDomainService : ICurrencyDomainService
    {
        /// <inheritdoc />
        public void ChangeDefaultCurrency(Currency previousDefaultCurrency, Currency newDefaultCurrency)
        {
            if (!previousDefaultCurrency.IsDefault)
            {
                throw new OperationNotAllowedException($"{nameof(previousDefaultCurrency)} is not default.");
            }

            previousDefaultCurrency.UnsetDefault();
            newDefaultCurrency.SetAsDefault();
        }
    }
}
