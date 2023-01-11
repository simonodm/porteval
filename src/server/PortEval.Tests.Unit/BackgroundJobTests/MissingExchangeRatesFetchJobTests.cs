using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Interfaces;
using PortEval.Application.Models.PriceFetcher;
using PortEval.BackgroundJobs;
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
        [Fact]
        public async Task Run_ImportsExchangeRatesRetrievedFromPriceFetcher()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var time = DateTime.UtcNow;
            var currency = new Currency("USD", "US Dollar", "US$", true);
            var firstTargetCurrency = new Currency("EUR", "European Euro", "€", false);
            var secondTargetCurrency = new Currency("CZK", "Czech Koruna", "Kč", false);
            var exchangeRates = new List<ExchangeRates>
            {
                fixture.Build<ExchangeRates>()
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

            var priceFetcher = CreatePriceFetcherReturningExchangeRates(fixture, exchangeRates);
            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Currency> { currency, firstTargetCurrency, secondTargetCurrency });
            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            exchangeRateRepository
                .Setup(m => m.ListExchangeRatesAsync(currency.Code))
                .ReturnsAsync(Enumerable.Empty<CurrencyExchangeRate>());

            // MissingExchangeRatesFetchJob clears the list after doing bulk insert, which makes it impossible to verify the correct invokation
            // afterwards. Instead we register a callback which sets the correct flag on invokation and check that flag using Assert
            exchangeRateRepository
                .Setup(m => m.BulkInsertAsync(It.Is<IList<CurrencyExchangeRate>>(list =>
                    list.Count == 2 &&
                    list.Any(er => er.Time == time && er.CurrencyToCode == firstTargetCurrency.Code && er.ExchangeRate == 1m) &&
                    list.Any(er => er.Time == time && er.CurrencyToCode == secondTargetCurrency.Code && er.ExchangeRate == 1m)
                )))
                .Returns(Task.Run(() => { }))
                .Callback(() => correctListSaved = true);

            var sut = fixture.Create<MissingExchangeRatesFetchJob>();

            await sut.Run();

            Assert.True(correctListSaved);
        }

        [Fact]
        public async Task Run_RetrievesOnlyMissingRanges()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

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
                fixture.Build<ExchangeRates>()
                    .With(er => er.Currency, currency.Code)
                    .With(er => er.Rates, new Dictionary<string, decimal>
                    {
                        { targetCurrency.Code, 1 }
                    })
                    .With(er => er.Time, newImportTime)
                    .Create()
            };

            var priceFetcher = CreatePriceFetcherReturningExchangeRates(fixture, newExchangeRates);
            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Currency> { currency, targetCurrency });
            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            exchangeRateRepository
                .Setup(m => m.ListExchangeRatesAsync(currency.Code))
                .ReturnsAsync(existingExchangeRates);

            var sut = fixture.Create<MissingExchangeRatesFetchJob>();

            await sut.Run();

            priceFetcher.Verify(m => m.GetHistoricalDailyExchangeRates(
                currency.Code,
                It.Is<DateTime>(dt => dt >= trackingStart),
                It.Is<DateTime>(dt => dt <= existingExchangeRateTime)
            ));
            priceFetcher.Verify(m => m.GetHistoricalDailyExchangeRates(
                currency.Code,
                It.Is<DateTime>(dt => dt >= trackingStart),
                It.IsAny<DateTime>()
            ));
        }

        [Fact]
        public async Task Run_ThrowsException_WhenNoDefaultCurrencyIsSet()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currency = new Currency("USD", "US Dollar", "US$", false);

            var priceFetcher = CreatePriceFetcherReturningExchangeRates(fixture, Enumerable.Empty<ExchangeRates>());
            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Currency> { currency });
            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();

            var sut = fixture.Create<MissingExchangeRatesFetchJob>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(sut.Run);
        }

        private Mock<IFinancialDataFetcher> CreatePriceFetcherReturningExchangeRates(IFixture fixture, IEnumerable<ExchangeRates> exchangeRates)
        {
            var priceFetcher = fixture.Freeze<Mock<IFinancialDataFetcher>>();
            priceFetcher
                .Setup(m => m.GetHistoricalDailyExchangeRates(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(exchangeRates);

            return priceFetcher;
        }
    }
}
