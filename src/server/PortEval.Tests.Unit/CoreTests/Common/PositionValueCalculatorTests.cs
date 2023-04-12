using PortEval.Application.Core.Common;
using PortEval.Application.Core.Common.Calculators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Common
{
    public class PositionValueCalculatorTests
    {
        [Fact]
        public void CalculateValue_ReturnsZero_WhenNoTransactionsArePresent()
        {
            var calculator = new PositionValueCalculator();

            var positionsPriceData = Enumerable.Empty<PositionPriceRangeData>();
            var valueTime = DateTime.Parse("2022-05-01");

            var value = calculator.CalculateValue(positionsPriceData, valueTime);

            Assert.Equal(0, value);
        }

        [Fact]
        public void CalculateValue_ReturnsInstrumentPriceAtRangeEnd_WhenOneTransactionIsPresent()
        {
            var calculator = new PositionValueCalculator();

            var priceAtRangeEnd = 150m;
            var valueTime = DateTime.Parse("2022-05-01");

            var positionPriceData = new PositionPriceRangeData()
            {
                PriceAtRangeEnd = new InstrumentPriceDto
                {
                    Time = valueTime.AddDays(-1),
                    Price = priceAtRangeEnd
                },
                DateRange = new DateRangeParams { To = valueTime },
                Transactions = new List<TransactionDto>
                {
                    new TransactionDto
                    {
                        Amount = 1,
                        Price = 113m,
                        Time = DateTime.Parse("2022-01-01")
                    }
                }
            };

            var value = calculator.CalculateValue(new[] { positionPriceData }, valueTime);

            Assert.Equal(priceAtRangeEnd, value);
        }

        [Fact]
        public void CalculateValue_ReturnsZero_WhenAmountAtRequestedTimeIsZero()
        {
            var calculator = new PositionValueCalculator();

            var valueTime = DateTime.Parse("2022-05-01");
            var priceAtValueTime = 150m;

            var positionPriceData = new PositionPriceRangeData
            {
                Transactions = new List<TransactionDto>
                {
                    new TransactionDto()
                    {
                        Amount = 1,
                        Price = 125m,
                        Time = DateTime.Parse("2022-01-01")
                    },
                    new TransactionDto
                    {
                        Amount = -1,
                        Price = 130m,
                        Time = DateTime.Parse("2022-05-01")
                    }
                },
                PriceAtRangeEnd = new InstrumentPriceDto
                {
                    Price = priceAtValueTime,
                    Time = valueTime.AddDays(-1)
                },
                DateRange = new DateRangeParams { To = valueTime }
            };

            var value = calculator.CalculateValue(new[] { positionPriceData }, valueTime);

            Assert.Equal(0, value);
        }

        [Fact]
        public void CalculateValue_ReturnsAccurateValue_Complex()
        {
            var calculator = new PositionValueCalculator();

            var valueTime = DateTime.Parse("2022-01-01");

            var firstPositionPriceData = new PositionPriceRangeData
            {
                Transactions = new List<TransactionDto>
                {
                    new TransactionDto()
                    {
                        Amount = 300,
                        Price = 88.17m,
                        Time = DateTime.Parse("2004-11-12")
                    },
                    new TransactionDto
                    {
                        Amount = -200,
                        Price = 189.11m,
                        Time = DateTime.Parse("2010-02-28")
                    },

                },
                PriceAtRangeEnd = new InstrumentPriceDto
                {
                    Price = 314.59m,
                    Time = valueTime
                },
                DateRange = new DateRangeParams { To = valueTime }
            };

            var secondPositionPriceData = new PositionPriceRangeData
            {
                Transactions = new List<TransactionDto>
                {
                    new TransactionDto
                    {
                        Amount = 700,
                        Price = 151.2m,
                        Time = DateTime.Parse("2008-05-14")
                    },

                    new TransactionDto
                    {
                        Amount = 100,
                        Price = 207.79m,
                        Time = DateTime.Parse("2017-03-15")
                    },
                    new TransactionDto
                    {
                        Amount = -100,
                        Price = 284.53m,
                        Time = DateTime.Parse("2019-05-14")
                    },
                    new TransactionDto
                    {
                        Amount = -50,
                        Price = 299.13m,
                        Time = DateTime.Parse("2021-10-04")
                    }
                },
                PriceAtRangeEnd = new InstrumentPriceDto
                {
                    Price = 241.13m,
                    Time = valueTime
                },
                DateRange = new DateRangeParams { To = valueTime }
            };

            var expectedValue = 188193.5m;

            var value = calculator.CalculateValue(new[] { firstPositionPriceData, secondPositionPriceData }, valueTime);

            Assert.Equal(expectedValue, value);
        }
    }
}
