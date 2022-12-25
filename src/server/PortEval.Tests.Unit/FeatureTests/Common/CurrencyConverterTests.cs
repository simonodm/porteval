using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Features.Common;
using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using PortEval.Domain.Exceptions;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Common
{
    public class CurrencyConverterTests
    {
        [Fact]
        public void CombineExchangeRates_CombinesExchangeRatesCorrectly()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var firstExchangeRates = new List<CurrencyExchangeRateDto>
            {
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2021-12-31")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-02")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-03")).Create(),
            };

            var secondExchangeRates = new List<CurrencyExchangeRateDto>
            {
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2021-12-31")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-01")).Create()
            };

            var sut = fixture.Create<CurrencyConverter>();

            var result = sut.CombineExchangeRates(firstExchangeRates, secondExchangeRates);

            Assert.Collection(result, rate =>
            {
                Assert.Equal(secondExchangeRates[0].Time, rate.Time);
                Assert.Equal(firstExchangeRates[0].ExchangeRate * secondExchangeRates[0].ExchangeRate, rate.ExchangeRate);
            }, rate =>
            {
                Assert.Equal(secondExchangeRates[1].Time, rate.Time);
                Assert.Equal(firstExchangeRates[0].ExchangeRate * secondExchangeRates[1].ExchangeRate, rate.ExchangeRate);
            });
        }

        [Fact]
        public void ConvertChartPoints_ConvertsChartPointsAccordingToProvidedExchangeRates()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartPoints = new List<EntityChartPointDto>
            {
                new (DateTime.Parse("2021-12-31"), fixture.Create<decimal>()),
                new (DateTime.Parse("2022-01-01"), fixture.Create<decimal>()),
                new (DateTime.Parse("2022-01-02"), fixture.Create<decimal>()),
                new (DateTime.Parse("2022-01-03"), fixture.Create<decimal>()),
                new (DateTime.Parse("2022-01-04"), fixture.Create<decimal>()),
            };

            var exchangeRates = new List<CurrencyExchangeRateDto>
            {
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2021-12-31")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-01")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-02")).Create(),
            };

            var sut = fixture.Create<CurrencyConverter>();

            var result = sut.ConvertChartPoints(chartPoints, exchangeRates);

            Assert.Collection(result, point =>
            {
                Assert.Equal(chartPoints[0].Value * exchangeRates[0].ExchangeRate, point.Value);
            }, point =>
            {
                Assert.Equal(chartPoints[1].Value * exchangeRates[1].ExchangeRate, point.Value);
            }, point =>
            {
                Assert.Equal(chartPoints[2].Value * exchangeRates[2].ExchangeRate, point.Value);
            }, point =>
            {
                Assert.Equal(chartPoints[3].Value * exchangeRates[2].ExchangeRate, point.Value);
            }, point =>
            {
                Assert.Equal(chartPoints[4].Value * exchangeRates[2].ExchangeRate, point.Value);
            });
        }

        [Fact]
        public void ConvertChartPoints_ThrowsException_WhenNoCurrencyExchangeRateIsAvailableForChartPoint()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartPoints = new List<EntityChartPointDto>
            {
                new (DateTime.Parse("2021-12-31"), fixture.Create<decimal>()),
                new (DateTime.Parse("2022-01-01"), fixture.Create<decimal>()),
                new (DateTime.Parse("2022-01-02"), fixture.Create<decimal>()),
                new (DateTime.Parse("2022-01-03"), fixture.Create<decimal>()),
                new (DateTime.Parse("2022-01-04"), fixture.Create<decimal>()),
            };

            var exchangeRates = new List<CurrencyExchangeRateDto>
            {
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-01")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-02")).Create(),
            };

            var sut = fixture.Create<CurrencyConverter>();

            Assert.Throws<ItemNotFoundException>(() => sut.ConvertChartPoints(chartPoints, exchangeRates));
        }

        [Fact]
        public void ConvertTransactions_ConvertsTransactionsAccordingToProvidedExchangeRates()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactions = new List<TransactionDto>
            {
                fixture.Build<TransactionDto>()
                    .With(t => t.Time, DateTime.Parse("2021-12-31"))
                    .With(t => t.Price, 100m)
                    .Create(),
                fixture.Build<TransactionDto>()
                    .With(t => t.Time, DateTime.Parse("2022-01-02 13:00"))
                    .With(t => t.Price, 120m)
                    .Create(),
            };

            var exchangeRates = new List<CurrencyExchangeRateDto>
            {
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2021-12-31")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-01")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-02")).Create(),
            };

            var sut = fixture.Create<CurrencyConverter>();

            var result = sut.ConvertTransactions(transactions, exchangeRates);

            Assert.Collection(result, t =>
            {
                Assert.Equal(transactions[0].Price * exchangeRates[0].ExchangeRate, t.Price);
            }, t =>
            {
                Assert.Equal(transactions[1].Price * exchangeRates[2].ExchangeRate, t.Price);
            });
        }

        [Fact]
        public void ConvertTransactions_ThrowsException_WhenNoCurrencyExchangeRateIsAvailableForTransaction()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactions = new List<TransactionDto>
            {
                fixture.Build<TransactionDto>()
                    .With(t => t.Time, DateTime.Parse("2021-12-31"))
                    .With(t => t.Price, 100m)
                    .Create()
            };

            var exchangeRates = new List<CurrencyExchangeRateDto>
            {
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-01")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-02")).Create(),
            };

            var sut = fixture.Create<CurrencyConverter>();

            Assert.Throws<ItemNotFoundException>(() => sut.ConvertTransactions(transactions, exchangeRates));
        }

        [Fact]
        public void ConvertInstrumentPrices_ConvertsPricesAccordingToProvidedExchangeRates()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var prices = new List<InstrumentPriceDto>
            {
                fixture.Build<InstrumentPriceDto>()
                    .With(t => t.Time, DateTime.Parse("2021-12-31"))
                    .With(t => t.Price, 100m)
                    .Create(),
                fixture.Build<InstrumentPriceDto>()
                    .With(t => t.Time, DateTime.Parse("2022-01-02 13:00"))
                    .With(t => t.Price, 120m)
                    .Create(),
            };

            var exchangeRates = new List<CurrencyExchangeRateDto>
            {
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2021-12-31")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-01")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-02")).Create(),
            };

            var sut = fixture.Create<CurrencyConverter>();

            var result = sut.ConvertInstrumentPrices(prices, exchangeRates);

            Assert.Collection(result, t =>
            {
                Assert.Equal(prices[0].Price * exchangeRates[0].ExchangeRate, t.Price);
            }, t =>
            {
                Assert.Equal(prices[1].Price * exchangeRates[2].ExchangeRate, t.Price);
            });
        }

        [Fact]
        public void ConvertInstrumentPrices_ThrowsException_WhenNoCurrencyExchangeRateIsAvailableForPrice()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var prices = new List<InstrumentPriceDto>
            {
                fixture.Build<InstrumentPriceDto>()
                    .With(t => t.Time, DateTime.Parse("2021-12-31"))
                    .With(t => t.Price, 100m)
                    .Create()
            };

            var exchangeRates = new List<CurrencyExchangeRateDto>
            {
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-01")).Create(),
                fixture.Build<CurrencyExchangeRateDto>().With(r => r.Time, DateTime.Parse("2022-01-02")).Create(),
            };

            var sut = fixture.Create<CurrencyConverter>();

            Assert.Throws<ItemNotFoundException>(() => sut.ConvertInstrumentPrices(prices, exchangeRates));
        }
    }
}
