using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.DomainEventHandlers.TransactionAddedToPosition;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Events;
using PortEval.Tests.Unit.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.DomainEventHandlers
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
            var priceService = fixture.Freeze<Mock<IInstrumentPriceService>>();
            priceService
                .Setup(m => m.AddPricePointAsync(It.IsAny<InstrumentPriceDto>()))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(fixture.Create<InstrumentPriceDto>()));
            priceService
                .Setup(m => m.GetInstrumentPriceAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<InstrumentPriceDto>());

            var sut = fixture.Create<CreateInstrumentPriceIfNotExistsWhenTransactionIsAddedDomainEventHandler>();

            await sut.Handle(domainEventAdapter, default);

            priceService.Verify(s =>
                s.AddPricePointAsync(It.Is<InstrumentPriceDto>(p =>
                    p.InstrumentId == domainEvent.Position.InstrumentId &&
                    p.Time == domainEvent.Transaction.Time &&
                    p.Price == domainEvent.Transaction.Price
                )
            ));
        }
    }
}
