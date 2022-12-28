using PortEval.Application.Features.Interfaces.Services;
using PortEval.Domain.Events;
using System.Threading;
using System.Threading.Tasks;
using PortEval.Application.Features.Interfaces;

namespace PortEval.Application.Features.DomainEventHandlers.TransactionChanged
{
    public class CreateInstrumentPriceIfNotExistsWhenTransactionIsUpdatedDomainEventHandler : IDomainEventHandler<TransactionChangedDomainEvent>
    {
        private readonly IInstrumentPriceService _priceService;

        public CreateInstrumentPriceIfNotExistsWhenTransactionIsUpdatedDomainEventHandler(
            IInstrumentPriceService priceService)
        {
            _priceService = priceService;
        }

        public async Task Handle(TransactionChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _priceService.AddPriceIfNotExistsAsync(notification.Position.InstrumentId,
                notification.Transaction.Time, notification.Transaction.Price);
        }
    }
}
