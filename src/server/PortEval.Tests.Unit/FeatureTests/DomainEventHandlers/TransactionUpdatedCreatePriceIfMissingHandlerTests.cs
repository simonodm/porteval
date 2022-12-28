using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Features.DomainEventHandlers.TransactionChanged;
using PortEval.Domain.Events;
using PortEval.Tests.Unit.Helpers.Extensions;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.DomainEventHandlers
{
    public class TransactionUpdatedCreatePriceIfMissingHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesInstrumentPriceAtTransactionTimeIfMissing()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            var domainEvent = fixture.Create<TransactionChangedDomainEvent>();
            var priceService = fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<CreateInstrumentPriceIfNotExistsWhenTransactionIsUpdatedDomainEventHandler>();

            await sut.Handle(domainEvent, default);

            priceService.Verify(s =>
                s.AddPriceIfNotExistsAsync(
                    domainEvent.Position.InstrumentId,
                    domainEvent.Transaction.Time,
                    domainEvent.Transaction.Price
                ));
        }
    }
}
