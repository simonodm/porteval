using AutoFixture;
using AutoFixture.AutoMoq;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using PortEval.Domain.Events;
using System.Threading.Tasks;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.DomainEventHandlers.InstrumentCreated;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.DomainEventHandlers
{
    public class InstrumentCreatedInitialPriceFetchHandlerTests
    {
        [Fact]
        public async Task
            Handle_CreatesInitialPriceFetchJobForInstrument_WhenInstrumentIsCreatedSuccessfully()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var domainEvent = fixture.Create<InstrumentCreatedDomainEvent>();
            var domainEventAdapter = new DomainEventNotificationAdapter<InstrumentCreatedDomainEvent>(domainEvent);
            var backgroundJobClient = fixture.Freeze<Mock<IBackgroundJobClient>>();
            fixture.CreateDefaultInstrumentRepositoryMock();

            var sut = fixture.Create<StartInitialPriceFetchWhenInstrumentCreatedDomainEventHandler>();

            await sut.Handle(domainEventAdapter, default);

            backgroundJobClient.Verify(c => c.Create(
                It.Is<Job>(job =>
                    job.Method.Name == "Run" && (int)job.Args[0] == domainEvent.Instrument.Id &&
                    job.Type.IsAssignableTo(typeof(IInitialPriceFetchJob))),
                It.IsAny<EnqueuedState>()
            ));
        }
    }
}
