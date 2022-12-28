using AutoFixture;
using AutoFixture.AutoMoq;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using PortEval.Application.Features.DomainEventHandlers.InstrumentSplitRollbackRequested;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.DomainEventHandlers
{
    public class InstrumentSplitRolledBackPriceAdjustmentJobHandlerTests
    {
        [Fact]
        public async Task Handle_EnqueuesPriceAdjustmentJob()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var domainEvent = fixture.Create<InstrumentSplitRollbackRequestedDomainEvent>();
            var jobClient = fixture.Freeze<Mock<IBackgroundJobClient>>();

            var sut = fixture.Create<AdjustPricesAndTransactionsWhenInstrumentSplitRolledBackDomainEventHandler>();

            await sut.Handle(domainEvent, default);

            jobClient.Verify(c => c.Create(
                It.Is<Job>(job =>
                    job.Method.Name == "Run" &&
                    job.Type.IsAssignableTo(typeof(ISplitPriceAndTransactionAdjustmentJob))),
                It.IsAny<EnqueuedState>()
            ));
        }
    }
}
