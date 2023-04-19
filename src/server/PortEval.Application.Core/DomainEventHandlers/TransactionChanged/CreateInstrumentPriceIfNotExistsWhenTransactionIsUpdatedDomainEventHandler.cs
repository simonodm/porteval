using System.Threading;
using System.Threading.Tasks;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Events;

namespace PortEval.Application.Core.DomainEventHandlers.TransactionChanged;

/// <summary>
///     A domain event handler creating a corresponding instrument price after a transaction is edited.
/// </summary>
public class
    CreateInstrumentPriceIfNotExistsWhenTransactionIsUpdatedDomainEventHandler : IDomainEventHandler<
        TransactionChangedDomainEvent>
{
    private readonly IInstrumentPriceService _priceService;

    /// <summary>
    ///     Initializes the domain event handler.
    /// </summary>
    public CreateInstrumentPriceIfNotExistsWhenTransactionIsUpdatedDomainEventHandler(
        IInstrumentPriceService priceService)
    {
        _priceService = priceService;
    }

    /// <summary>
    ///     Handles the domain event.
    /// </summary>
    public async Task Handle(DomainEventNotificationAdapter<TransactionChangedDomainEvent> notification,
        CancellationToken cancellationToken)
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