using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Services
{
    public interface ICurrencyDomainService
    {
        public void ChangeDefaultCurrency(Currency previousDefaultCurrency, Currency newDefaultCurrency);
    }
}
