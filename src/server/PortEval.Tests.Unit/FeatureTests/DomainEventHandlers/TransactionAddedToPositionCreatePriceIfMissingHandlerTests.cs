using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Features.DomainEventHandlers.TransactionAddedToPosition;
using PortEval.Domain.Events;
using PortEval.Tests.Unit.Helpers.Extensions;
using System.Threading.Tasks;
using PortEval.Application.Features.Common;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.DomainEventHandlers
{
    public class TransactionAddedToPositionCreatePriceIfMissingHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesInstrumentPriceAtTransactionTimeIfMissing()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var domainEvent = fixture.Create<TransactionAddedToPositionDomainEvent>();
            var domainEventAdapter =
                new DomainEventNotificationAdapter<TransactionAddedToPositionDomainEvent>(domainEvent);
            var priceService = fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<CreateInstrumentPriceIfNotExistsWhenTransactionIsAddedDomainEventHandler>();

            await sut.Handle(domainEventAdapter, default);

            priceService.Verify(s => 
                s.AddPriceIfNotExistsAsync(
                    domainEvent.Position.InstrumentId,
                    domainEvent.Transaction.Time,
                    domainEvent.Transaction.Price
            ));
        }
    }
}
