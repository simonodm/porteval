﻿using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.DomainEventHandlers.TransactionChanged;
using PortEval.Domain.Events;
using PortEval.Tests.Unit.Helpers.Extensions;
using System.Threading.Tasks;
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
            var domainEventAdapter =
                new DomainEventNotificationAdapter<TransactionChangedDomainEvent>(domainEvent);
            var priceService = fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<CreateInstrumentPriceIfNotExistsWhenTransactionIsUpdatedDomainEventHandler>();

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
