﻿using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.DomainEventHandlers.InstrumentSplitCreated;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.DomainEventHandlers;

public class InstrumentSplitCreatedPriceAdjustmentJobHandlerTests
{
    [Fact]
    public async Task Handle_EnqueuesSplitPriceAdjustmentJob()
    {
        var fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        var domainEvent = fixture.Create<InstrumentSplitCreatedDomainEvent>();
        var domainEventAdapter = new DomainEventNotificationAdapter<InstrumentSplitCreatedDomainEvent>(domainEvent);
        var jobClient = fixture.Freeze<Mock<IBackgroundJobClient>>();

        var sut = fixture.Create<AdjustPricesAndTransactionsWhenInstrumentSplitCreatedDomainEventHandler>();

        await sut.Handle(domainEventAdapter, default);

        jobClient.Verify(c => c.Create(
            It.Is<Job>(job =>
                job.Method.Name == nameof(ISplitPriceAndTransactionAdjustmentJob.RunAsync) &&
                job.Type.IsAssignableTo(typeof(ISplitPriceAndTransactionAdjustmentJob))),
            It.IsAny<EnqueuedState>()
        ));
    }
}