using AutoFixture;
using Moq;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;

namespace PortEval.Tests.Extensions
{
    internal static class FixtureExtensions
    {
        public static Mock<ICurrencyRepository> GetDefaultICurrencyRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<ICurrencyRepository>>();
            mock
                .Setup(m => m.FindAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(fixture.Create<Currency>()));
            mock
                .Setup(m => m.Exists(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Update(It.IsAny<Currency>()))
                .Returns<Currency>(c => c);
            mock
                .Setup(m => m.ListAllAsync())
                .Returns(Task.FromResult(fixture.CreateMany<Currency>()));
            mock
                .Setup(m => m.GetDefaultCurrencyAsync())
                .Returns(() =>
                {
                    var currency = fixture.Create<Currency>();
                    currency.SetAsDefault();
                    return Task.FromResult(currency);
                });

            return mock;
        }

        public static Mock<IExchangeRepository> GetDefaultIExchangeRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IExchangeRepository>>();
            mock
                .Setup(m => m.ListAllAsync())
                .Returns(Task.FromResult(fixture.CreateMany<Exchange>()));
            mock
                .Setup(m => m.FindAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(fixture.Create<Exchange>()));
            mock
                .Setup(m => m.Exists(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Add(It.IsAny<Exchange>()))
                .Returns<Exchange>(e => e);

            return mock;
        }

        public static Mock<IInstrumentRepository> GetDefaultIInstrumentRepositoryMock(this IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IInstrumentRepository>>();
            mock
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fixture.Create<Instrument>()));
            mock
                .Setup(m => m.Exists(It.IsAny<int>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.Add(It.IsAny<Instrument>()))
                .Returns<Instrument>(i => i);
            mock
                .Setup(m => m.Update(It.IsAny<Instrument>()))
                .Returns<Instrument>(i => i);
            mock
                .Setup(m => m.ListAllAsync())
                .Returns(Task.FromResult(fixture.CreateMany<Instrument>()));

            return mock;
        }
    }
}
