using AutoFixture;
using AutoFixture.AutoMoq;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.DomainEventHandlers.DefaultCurrencyChanged;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.DomainEventHandlers
{
    public class DefaultCurrencyChangedMissingExchangeRatesHandlerTests
    {
        [Fact]
        public async Task Handle_EnqueuesMissingExchangeRateFetchJob()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var domainEvent = fixture.Create<DefaultCurrencyChangedDomainEvent>();
            var domainEventAdapter = new DomainEventNotificationAdapter<DefaultCurrencyChangedDomainEvent>(domainEvent);
            var backgroundJobClient = fixture.Freeze<Mock<IBackgroundJobClient>>();

            var sut = fixture.Create<MissingExchangeRatesFetchWhenDefaultCurrencyChangesDomainEventHandler>();

            await sut.Handle(domainEventAdapter, default);

            backgroundJobClient.Verify(c => c.Create(
                It.Is<Job>(job =>
                    job.Method.Name == nameof(IMissingExchangeRatesFetchJob.RunAsync) &&
                    job.Type.IsAssignableTo(typeof(IMissingExchangeRatesFetchJob))),
                It.IsAny<EnqueuedState>()
            ));
        }
    }
}
