using PortEval.Application.Core.Common;
using PortEval.Application.Core.Common.Calculators;
using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Common
{
    public class PositionProfitCalculatorTests
    {
        [Fact]
        public void CalculateProfit_ReturnsZero_WhenNoTransactionsArePresent()
        {
            var calculator = new PositionProfitCalculator();

            var positionsPriceData = Enumerable.Empty<PositionPriceRangeData>();
            var from = DateTime.Parse("2022-01-01");
            var to = DateTime.Parse("2022-04-01");

            var profit = calculator.CalculateProfit(positionsPriceData, from, to);

            Assert.Equal(0, profit);
        }

        [Fact]
        public void CalculateProfit_ReturnsDifferenceBetweenRangeEndPriceAndTransactionPrice_WhenOneTransactionIsPresent()
        {
            var calculator = new PositionProfitCalculator();

            var priceAtRangeEnd = 189.12m;
            var transactionPrice = 155.14m;
            var transactionTime = DateTime.Parse("2022-04-01");

            var from = DateTime.Parse("2022-01-01");
            var to = DateTime.Parse("2022-05-01");

            var positionPriceData = new PositionPriceRangeData
            {
                Transactions = new[]
                {
                    new TransactionDto
                    {
                        Amount = 1,
                        Price = transactionPrice,
                        Time = transactionTime
                    }
                },
                PriceAtRangeStart = new InstrumentPriceDto
                {
                    Price = priceAtRangeEnd / 2,
                    Time = from
                },
                PriceAtRangeEnd = new InstrumentPriceDto
                {
                    Price = priceAtRangeEnd,
                    Time = to
                }
            };

            var profit = calculator.CalculateProfit(new[] { positionPriceData }, from, to);

            Assert.Equal(priceAtRangeEnd - transactionPrice, profit);
        }

        [Fact]
        public void CalculateProfit_ReturnsAccurateProfit_Complex()
        {
            var from = DateTime.Parse("2006-05-01");
            var to = DateTime.Parse("2010-01-01");

            var firstPositionPriceData = new PositionPriceRangeData
            {
                Transactions = new List<TransactionDto>
                {
                    new TransactionDto
                    {
                        Amount = 400,
                        Price = 155.66m,
                        Time = DateTime.Parse("2004-05-01")
                    },
                    new TransactionDto
                    {
                        Amount = -100,
                        Price = 193.57m,
                        Time = DateTime.Parse("2006-02-13")
                    },
                    new TransactionDto
                    {
                        Amount = -200,
                        Price = 241.51m,
                        Time = DateTime.Parse("2009-06-30")
                    }
                },
                PriceAtRangeStart = new InstrumentPriceDto
                {
                    Price = 145.14m,
                    Time = from
                },
                PriceAtRangeEnd = new InstrumentPriceDto
                {
                    Price = 394.29m,
                    Time = to
                }
            };

            var secondPositionPriceData = new PositionPriceRangeData
            {
                Transactions = new List<TransactionDto>
                {
                    new TransactionDto
                    {
                        Amount = 600,
                        Price = 16.59m,
                        Time = DateTime.Parse("2006-07-11")
                    },
                    new TransactionDto
                    {
                        Amount = -455,
                        Price = 15.42m,
                        Time = DateTime.Parse("2007-08-27")
                    },
                },
                PriceAtRangeStart = new InstrumentPriceDto
                {
                    Price = 17.55m,
                    Time = from
                },
                PriceAtRangeEnd = new InstrumentPriceDto
                {
                    Price = 15.44m,
                    Time = to
                }
            };

            var expectedProfit = 43489.9m;

            var calculator = new PositionProfitCalculator();
            var profit = calculator.CalculateProfit(new[] { firstPositionPriceData, secondPositionPriceData }, from, to);

            Assert.Equal(expectedProfit, profit);
        }
    }
}
