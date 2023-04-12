using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.BackgroundJobs;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests
{
    public class MissingExchangeRatesFetchJobTests
    {
        private IFixture _fixture;
        private Mock<ICurrencyRepository> _currencyRepository;
        private Mock<ICurrencyExchangeRateRepository> _exchangeRateRepository;

        public MissingExchangeRatesFetchJobTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _currencyRepository = _fixture.CreateDefaultCurrencyRepositoryMock();
            _exchangeRateRepository = _fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
        }

        [Fact]
        public async Task Run_ImportsExchangeRatesRetrievedFromPriceFetcher()
        {
            var time = DateTime.UtcNow;
            var currency = new Currency("USD", "US Dollar", "US$", true);
            var firstTargetCurrency = new Currency("EUR", "European Euro", "€", false);
            var secondTargetCurrency = new Currency("CZK", "Czech Koruna", "Kč", false);
            var exchangeRates = new List<ExchangeRates>
            {
                _fixture.Build<ExchangeRates>()
                    .With(er => er.Currency, currency.Code)
                    .With(er => er.Rates, new Dictionary<string, decimal>
                    {
                        { firstTargetCurrency.Code, 1 },
                        { secondTargetCurrency.Code, 1 }
                    })
                    .With(er => er.Time, time)
                    .Create()
            };

            var correctListSaved = false;

            CreatePriceFetcherReturningExchangeRates(_fixture, exchangeRates);
            _currencyRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Currency> { currency, firstTargetCurrency, secondTargetCurrency });
            _exchangeRateRepository
                .Setup(m => m.ListExchangeRatesAsync(currency.Code))
                .ReturnsAsync(Enumerable.Empty<CurrencyExchangeRate>());

            // MissingExchangeRatesFetchJob clears the list after doing bulk insert, which makes it impossible to verify the correct invokation
            // afterwards. Instead we register a callback which sets the correct flag on invokation and check that flag using Assert
            _exchangeRateRepository
                .Setup(m => m.BulkUpsertAsync(It.Is<IList<CurrencyExchangeRate>>(list =>
                    list.Count == 2 &&
                    list.Any(er => er.Time == time && er.CurrencyToCode == firstTargetCurrency.Code && er.ExchangeRate == 1m) &&
                    list.Any(er => er.Time == time && er.CurrencyToCode == secondTargetCurrency.Code && er.ExchangeRate == 1m)
                )))
                .Returns(Task.Run(() => { }))
                .Callback(() => correctListSaved = true);

            var sut = _fixture.Create<MissingExchangeRatesFetchJob>();

            await sut.RunAsync();

            Assert.True(correctListSaved);
        }

        [Fact]
        public async Task Run_RetrievesOnlyMissingRanges()
        {
            var trackingStart = DateTime.Parse("2022-01-09");
            var existingExchangeRateTime = trackingStart.AddDays(2);
            var newImportTime = trackingStart.AddDays(1);

            var currency = new Currency("USD", "US Dollar", "US$", true);
            currency.SetTrackingFrom(trackingStart);
            var targetCurrency = new Currency("EUR", "European Euro", "€", false);
            var existingExchangeRates = new List<CurrencyExchangeRate>
            {
                new CurrencyExchangeRate(1, existingExchangeRateTime, 1m, currency.Code, targetCurrency.Code)
            };
            var newExchangeRates = new List<ExchangeRates>
            {
                _fixture.Build<ExchangeRates>()
                    .With(er => er.Currency, currency.Code)
                    .With(er => er.Rates, new Dictionary<string, decimal>
                    {
                        { targetCurrency.Code, 1 }
                    })
                    .With(er => er.Time, newImportTime)
                    .Create()
            };

            var priceFetcher = CreatePriceFetcherReturningExchangeRates(_fixture, newExchangeRates);
            _currencyRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Currency> { currency, targetCurrency });
            _exchangeRateRepository
                .Setup(m => m.ListExchangeRatesAsync(currency.Code))
                .ReturnsAsync(existingExchangeRates);

            var sut = _fixture.Create<MissingExchangeRatesFetchJob>();

            await sut.RunAsync();

            priceFetcher.Verify(m => m.GetHistoricalDailyExchangeRatesAsync(
                currency.Code,
                It.Is<DateTime>(dt => dt >= trackingStart),
                It.Is<DateTime>(dt => dt <= existingExchangeRateTime)
            ));
            priceFetcher.Verify(m => m.GetHistoricalDailyExchangeRatesAsync(
                currency.Code,
                It.Is<DateTime>(dt => dt >= trackingStart),
                It.IsAny<DateTime>()
            ));
        }

        [Fact]
        public async Task Run_ThrowsException_WhenNoDefaultCurrencyIsSet()
        {
            var _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currency = new Currency("USD", "US Dollar", "US$", false);

            CreatePriceFetcherReturningExchangeRates(_fixture, Enumerable.Empty<ExchangeRates>());
            _currencyRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Currency> { currency });

            var sut = _fixture.Create<MissingExchangeRatesFetchJob>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(sut.RunAsync);
        }

        private Mock<IFinancialDataFetcher> CreatePriceFetcherReturningExchangeRates(IFixture _fixture, IEnumerable<ExchangeRates> exchangeRates)
        {
            var priceFetcher = _fixture.Freeze<Mock<IFinancialDataFetcher>>();
            priceFetcher
                .Setup(m => m.GetHistoricalDailyExchangeRatesAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(exchangeRates);

            return priceFetcher;
        }
    }
}
