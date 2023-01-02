using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events
{
    public class DefaultCurrencyChangedDomainEvent : IDomainEvent
    {
        public Currency NewDefaultCurrency { get; init; }

        public DefaultCurrencyChangedDomainEvent(Currency newDefaultCurrency)
        {
            NewDefaultCurrency = newDefaultCurrency;
        }
    }
}
