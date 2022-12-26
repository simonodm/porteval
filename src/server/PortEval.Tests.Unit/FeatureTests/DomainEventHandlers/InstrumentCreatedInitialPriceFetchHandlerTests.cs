using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using PortEval.Application.Features.DomainEventHandlers.InstrumentCreated;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
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

            var backgroundJobClient = fixture.Freeze<Mock<IBackgroundJobClient>>();

            var sut = fixture.Create<StartInitialPriceFetchWhenInstrumentCreatedDomainEventHandler>();

            await sut.Handle(domainEvent, default);

            backgroundJobClient.Verify(c => c.Create(
                It.Is<Job>(job =>
                    job.Method.Name == "Run" && (int)job.Args[0] == domainEvent.Instrument.Id &&
                    job.Type.IsAssignableTo(typeof(IInitialPriceFetchJob))),
                It.IsAny<EnqueuedState>()
            ));
        }
    }
}
