using PortEval.Application.Services.Queries.Calculators;
using PortEval.Application.Services.Queries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PortEval.Tests.UnitTests.Queries.Calculators
{
    public class TransactionBasedProfitCalculatorTests
    {
        [Fact]
        public void CalculateProfit_ReturnsZero_WhenNoTransactionsArePresent()
        {
            var calculator = new TransactionBasedProfitCalculator();

            var transactions = Enumerable.Empty<TransactionDetailsQueryModel>();
            var from = DateTime.Parse("2022-01-01");
            var to = DateTime.Parse("2022-04-01");

            var profit = calculator.CalculateProfit(transactions, from, to);

            Assert.Equal(0, profit);
        }

        [Fact]
        public void CalculateProfit_ReturnsDifferenceBetweenRangeEndPriceAndTransactionPrice_WhenOneTransactionIsPresent()
        {
            var calculator = new TransactionBasedProfitCalculator();

            var priceAtRangeEnd = 189.12m;
            var transactionPrice = 155.14m;
            var transactionTime = DateTime.Parse("2022-04-01");

            var transaction = new TransactionDetailsQueryModel
            {
                Amount = 1,
                Price = transactionPrice,
                InstrumentPriceAtRangeStart = priceAtRangeEnd / 2,
                InstrumentPriceAtRangeEnd = priceAtRangeEnd,
                Time = transactionTime
            };

            var from = DateTime.Parse("2022-01-01");
            var to = DateTime.Parse("2022-05-01");

            var transactions = new List<TransactionDetailsQueryModel> { transaction };

            var profit = calculator.CalculateProfit(transactions, from, to);

            Assert.Equal(priceAtRangeEnd - transactionPrice, profit);
        }

        [Fact]
        public void CalculateProfit_ReturnsAccurateProfit_Complex()
        {
            var calculator = new TransactionBasedProfitCalculator();

            var transactions = new List<TransactionDetailsQueryModel>
            {
                new TransactionDetailsQueryModel
                {
                    Amount = 400,
                    InstrumentPriceAtRangeStart = 145.14m,
                    InstrumentPriceAtRangeEnd = 394.29m,
                    Price = 155.66m,
                    Time = DateTime.Parse("2004-05-01")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -100,
                    InstrumentPriceAtRangeStart = 145.14m,
                    InstrumentPriceAtRangeEnd = 394.29m,
                    Price = 193.57m,
                    Time = DateTime.Parse("2006-02-13")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = 600,
                    InstrumentPriceAtRangeStart = 17.55m,
                    InstrumentPriceAtRangeEnd = 15.44m,
                    Price = 16.59m,
                    Time = DateTime.Parse("2006-07-11")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -455,
                    InstrumentPriceAtRangeStart = 17.55m,
                    InstrumentPriceAtRangeEnd = 15.44m,
                    Price = 15.42m,
                    Time = DateTime.Parse("2007-08-27")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -200,
                    InstrumentPriceAtRangeStart = 145.14m,
                    InstrumentPriceAtRangeEnd = 394.29m,
                    Price = 241.51m,
                    Time = DateTime.Parse("2009-06-30")
                }
            };

            var from = DateTime.Parse("2006-05-01");
            var to = DateTime.Parse("2010-01-01");

            var expectedProfit = 43489.9m;

            var profit = calculator.CalculateProfit(transactions, from, to);

            Assert.Equal(expectedProfit, profit);
        }
    }
}
