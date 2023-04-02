using System.Threading;
using System.Threading.Tasks;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Domain.Events;

namespace PortEval.Application.Core.DomainEventHandlers.TransactionAddedToPosition
{
    public class CreateInstrumentPriceIfNotExistsWhenTransactionIsAddedDomainEventHandler : IDomainEventHandler<TransactionAddedToPositionDomainEvent>
    {
        private readonly IInstrumentPriceService _priceService;

        public CreateInstrumentPriceIfNotExistsWhenTransactionIsAddedDomainEventHandler(
            IInstrumentPriceService priceService)
        {
            _priceService = priceService;
        }

        public async Task Handle(DomainEventNotificationAdapter<TransactionAddedToPositionDomainEvent> notification, CancellationToken cancellationToken)
        {
            await _priceService.AddPriceIfNotExistsAsync(notification.DomainEvent.Position.InstrumentId,
                notification.DomainEvent.Transaction.Time, notification.DomainEvent.Transaction.Price);
        }
    }
}
