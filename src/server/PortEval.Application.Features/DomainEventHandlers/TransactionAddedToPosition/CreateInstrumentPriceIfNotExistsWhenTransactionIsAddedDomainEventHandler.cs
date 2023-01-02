using PortEval.Application.Features.Interfaces;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace PortEval.Application.Features.DomainEventHandlers.TransactionAddedToPosition
{
    public class CreateInstrumentPriceIfNotExistsWhenTransactionIsAddedDomainEventHandler : IDomainEventHandler<TransactionAddedToPositionDomainEvent>
    {
        private readonly IInstrumentPriceService _priceService;

        public CreateInstrumentPriceIfNotExistsWhenTransactionIsAddedDomainEventHandler(
            IInstrumentPriceService priceService)
        {
            _priceService = priceService;
        }

        public async Task Handle(TransactionAddedToPositionDomainEvent notification, CancellationToken cancellationToken)
        {
            await _priceService.AddPriceIfNotExistsAsync(notification.Position.InstrumentId,
                notification.Transaction.Time, notification.Transaction.Price);
        }
    }
}
