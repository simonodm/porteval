using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.DomainEventHandlers.InstrumentCreated;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Events;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.DomainEventHandlers
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
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.Update(It.IsAny<Instrument>()))
                .Returns<Instrument>(i => i);

            var sut = fixture.Create<StartInitialPriceFetchWhenInstrumentCreatedDomainEventHandler>();

            await sut.Handle(domainEventAdapter, default);

            backgroundJobClient.Verify(c => c.Create(
                It.Is<Job>(job =>
                    job.Method.Name == nameof(IInitialPriceFetchJob.RunAsync) &&
                    (int)job.Args[0] == domainEvent.Instrument.Id &&
                    job.Type.IsAssignableTo(typeof(IInitialPriceFetchJob))),
                It.IsAny<EnqueuedState>()
            ));
        }
    }
}
