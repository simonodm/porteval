using AutoFixture;
using AutoFixture.AutoMoq;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.DomainEventHandlers.InstrumentSplitRollbackRequested;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.DomainEventHandlers
{
    public class InstrumentSplitRolledBackPriceAdjustmentJobHandlerTests
    {
        [Fact]
        public async Task Handle_EnqueuesPriceAdjustmentJob()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var domainEvent = fixture.Create<InstrumentSplitRollbackRequestedDomainEvent>();
            var domainEventAdapter =
                new DomainEventNotificationAdapter<InstrumentSplitRollbackRequestedDomainEvent>(domainEvent);
            var jobClient = fixture.Freeze<Mock<IBackgroundJobClient>>();

            var sut = fixture.Create<AdjustPricesAndTransactionsWhenInstrumentSplitRolledBackDomainEventHandler>();

            await sut.Handle(domainEventAdapter, default);

            jobClient.Verify(c => c.Create(
                It.Is<Job>(job =>
                    job.Method.Name == nameof(ISplitPriceAndTransactionAdjustmentJob.RunAsync) &&
                    job.Type.IsAssignableTo(typeof(ISplitPriceAndTransactionAdjustmentJob))),
                It.IsAny<EnqueuedState>()
            ));
        }
    }
}
