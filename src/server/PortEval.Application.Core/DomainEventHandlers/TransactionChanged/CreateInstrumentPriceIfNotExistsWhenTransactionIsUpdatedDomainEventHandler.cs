using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

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
            var existingPrice = await _priceService.GetInstrumentPriceAsync(
                notification.DomainEvent.Position.InstrumentId, notification.DomainEvent.Transaction.Time);

            if (existingPrice.Status == OperationStatus.NotFound)
            {
                var newPrice = new InstrumentPriceDto
                {
                    InstrumentId = notification.DomainEvent.Position.InstrumentId,
                    Time = notification.DomainEvent.Transaction.Time,
                    Price = notification.DomainEvent.Transaction.Price
                };

                await _priceService.AddPricePointAsync(newPrice);
            }
        }
    }
}
