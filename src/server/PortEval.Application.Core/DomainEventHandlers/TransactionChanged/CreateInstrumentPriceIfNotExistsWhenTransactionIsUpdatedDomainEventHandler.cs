using System.Threading;
using System.Threading.Tasks;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Domain.Events;

namespace PortEval.Application.Core.DomainEventHandlers.TransactionChanged
{
    public class CreateInstrumentPriceIfNotExistsWhenTransactionIsUpdatedDomainEventHandler : IDomainEventHandler<TransactionChangedDomainEvent>
    {
        private readonly IInstrumentPriceService _priceService;

        public CreateInstrumentPriceIfNotExistsWhenTransactionIsUpdatedDomainEventHandler(
            IInstrumentPriceService priceService)
        {
            _priceService = priceService;
        }

        public async Task Handle(DomainEventNotificationAdapter<TransactionChangedDomainEvent> notification, CancellationToken cancellationToken)
        {
            await _priceService.AddPriceIfNotExistsAsync(notification.DomainEvent.Position.InstrumentId,
                notification.DomainEvent.Transaction.Time, notification.DomainEvent.Transaction.Price);
        }
    }
}
