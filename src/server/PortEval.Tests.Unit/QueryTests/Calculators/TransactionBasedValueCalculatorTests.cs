using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Services.Queries.Calculators;
using PortEval.Application.Services.Queries.Models;
using Xunit;

namespace PortEval.Tests.Unit.QueryTests.Calculators
{
    public class TransactionBasedValueCalculatorTests
    {
        [Fact]
        public void CalculateValue_ReturnsZero_WhenNoTransactionsArePresent()
        {
            var calculator = new TransactionBasedValueCalculator();

            var transactions = Enumerable.Empty<TransactionDetailsQueryModel>();
            var valueTime = DateTime.Parse("2022-05-01");

            var value = calculator.CalculateValue(transactions, valueTime);

            Assert.Equal(0, value);
        }

        [Fact]
        public void CalculateValue_ReturnsInstrumentPriceAtRangeEnd_WhenOneTransactionIsPresent()
        {
            var calculator = new TransactionBasedValueCalculator();

            var priceAtRangeEnd = 150m;

            var transaction = new TransactionDetailsQueryModel
            {
                Amount = 1,
                InstrumentPriceAtRangeEnd = priceAtRangeEnd,
                InstrumentPriceAtRangeStart = priceAtRangeEnd / 2,
                Price = 113m,
                Time = DateTime.Parse("2022-01-01")
            };

            var valueTime = DateTime.Parse("2022-05-01");
            var transactions = new List<TransactionDetailsQueryModel> { transaction };

            var value = calculator.CalculateValue(transactions, valueTime);

            Assert.Equal(priceAtRangeEnd, value);
        }

        [Fact]
        public void CalculateValue_ReturnsZero_WhenAmountAtRequestedTimeIsZero()
        {
            var calculator = new TransactionBasedValueCalculator();

            var firstTransaction = new TransactionDetailsQueryModel
            {
                Amount = 1,
                InstrumentPriceAtRangeStart = 100m,
                InstrumentPriceAtRangeEnd = 150m,
                Price = 125m,
                Time = DateTime.Parse("2022-01-01")
            };

            var secondTransaction = new TransactionDetailsQueryModel
            {
                Amount = -1,
                InstrumentPriceAtRangeStart = 100m,
                InstrumentPriceAtRangeEnd = 150m,
                Price = 130m,
                Time = DateTime.Parse("2022-05-01")
            };

            var valueTime = DateTime.Parse("2022-05-01");
            var transactions = new List<TransactionDetailsQueryModel> { firstTransaction, secondTransaction };

            var value = calculator.CalculateValue(transactions, valueTime);

            Assert.Equal(0, value);
        }

        [Fact]
        public void CalculateValue_ReturnsAccurateValue_Complex()
        {
            var calculator = new TransactionBasedValueCalculator();

            var transactions = new List<TransactionDetailsQueryModel>
            {
                new TransactionDetailsQueryModel
                {
                    Amount = 300,
                    InstrumentPriceAtRangeStart = 84.12m,
                    InstrumentPriceAtRangeEnd = 314.59m,
                    Price = 88.17m,
                    Time = DateTime.Parse("2004-11-12")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = 700,
                    InstrumentPriceAtRangeStart = 184.15m,
                    InstrumentPriceAtRangeEnd = 241.13m,
                    Price = 151.2m,
                    Time = DateTime.Parse("2008-05-14")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -200,
                    InstrumentPriceAtRangeStart = 84.12m,
                    InstrumentPriceAtRangeEnd = 314.59m,
                    Price = 189.11m,
                    Time = DateTime.Parse("2010-02-28")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = 100,
                    InstrumentPriceAtRangeStart = 184.15m,
                    InstrumentPriceAtRangeEnd = 241.13m,
                    Price = 207.79m,
                    Time = DateTime.Parse("2017-03-15")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -100,
                    InstrumentPriceAtRangeStart = 184.15m,
                    InstrumentPriceAtRangeEnd = 241.13m,
                    Price = 284.53m,
                    Time = DateTime.Parse("2019-05-14")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -50,
                    InstrumentPriceAtRangeStart = 184.15m,
                    InstrumentPriceAtRangeEnd = 241.13m,
                    Price = 299.13m,
                    Time = DateTime.Parse("2021-10-04")
                }
            };

            var valueTime = DateTime.Parse("2022-01-01");
            var expectedValue = 188193.5m;

            var value = calculator.CalculateValue(transactions, valueTime);

            Assert.Equal(expectedValue, value);
        }
    }
}
