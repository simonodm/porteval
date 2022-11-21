using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Services.Queries.Calculators;
using PortEval.Application.Services.Queries.Models;
using Xunit;

namespace PortEval.Tests.Unit.QueryTests.Calculators
{
    public class IrrPerformanceCalculatorTests
    {
        [Theory]
        [MemberData(nameof(GenerateTestData))]
        public void CalculateIrr_ReturnsCorrectRateOfReturn(IEnumerable<TransactionDetailsQueryModel> transactions, DateTime start, DateTime end, decimal expectedIrr)
        {
            var calculator = new IrrPerformanceCalculator();

            var result = calculator.CalculatePerformance(transactions, start, end);

            Assert.InRange(result, expectedIrr - 0.01m, expectedIrr + 0.01m);
        }

        [Fact]
        public void CalculateIrr_ReturnsZeroIrr_WhenNoTransactionsAreProvided()
        {
            var calculator = new IrrPerformanceCalculator();

            var result = calculator.CalculatePerformance(Enumerable.Empty<TransactionDetailsQueryModel>(),
                DateTime.MinValue, DateTime.MaxValue);

            Assert.Equal(0, result);
        }

        public static IEnumerable<object[]> GenerateTestData()
        {
            yield return GenerateTestData_OnePurchaseTransaction();
            yield return GenerateTestData_WithSaleTransaction_PositiveIrr();
            yield return GenerateTestData_WithSaleTransaction_NegativeIrr();
            yield return GenerateTestData_MultipleTransactions_PositiveIrr();
            yield return GenerateTestData_MultipleTransactions_NegativeIrr();
            yield return GenerateTestData_MultipleTransactions_Complex();
        }

        private static object[] GenerateTestData_OnePurchaseTransaction()
        {
            var transactions = new List<TransactionDetailsQueryModel>
            {
                new TransactionDetailsQueryModel
                {
                    Amount = 1m,
                    InstrumentPriceAtRangeStart = 100m,
                    InstrumentPriceAtRangeEnd = 125m,
                    Price = 113m,
                    Time = DateTime.Parse("2022-02-01 14:00")
                },
            };

            var start = DateTime.Parse("2021-12-25");
            var end = DateTime.Parse("2022-03-01");

            var expected = 0.106m;

            return new object[] { transactions, start, end, expected };
        }

        private static object[] GenerateTestData_WithSaleTransaction_PositiveIrr()
        {
            var transactions = new List<TransactionDetailsQueryModel>
            {
                new TransactionDetailsQueryModel
                {
                    Amount = 1m,
                    InstrumentPriceAtRangeStart = 97.14m,
                    InstrumentPriceAtRangeEnd = 103.12m,
                    Price = 92.14m,
                    Time = DateTime.Parse("2020-06-16 15:35")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -1m,
                    InstrumentPriceAtRangeStart = 97.14m,
                    InstrumentPriceAtRangeEnd = 103.12m,
                    Price = 99.13m,
                    Time = DateTime.Parse("2020-07-01 16:24")
                }
            };

            var start = DateTime.Parse("2020-05-15");
            var end = DateTime.Parse("2020-07-15");

            var expected = 0.157m;

            return new object[] { transactions, start, end, expected };
        }

        private static object[] GenerateTestData_WithSaleTransaction_NegativeIrr()
        {
            var transactions = new List<TransactionDetailsQueryModel>
            {
                new TransactionDetailsQueryModel
                {
                    Amount = 1m,
                    InstrumentPriceAtRangeStart = 97.14m,
                    InstrumentPriceAtRangeEnd = 103.12m,
                    Price = 92.14m,
                    Time = DateTime.Parse("2020-06-16 15:35")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -1m,
                    InstrumentPriceAtRangeStart = 97.14m,
                    InstrumentPriceAtRangeEnd = 103.12m,
                    Price = 90.55m,
                    Time = DateTime.Parse("2020-07-01 16:24")
                }
            };

            var start = DateTime.Parse("2020-05-15");
            var end = DateTime.Parse("2020-07-15");

            var expected = -0.034m;

            return new object[] { transactions, start, end, expected };
        }

        private static object[] GenerateTestData_MultipleTransactions_PositiveIrr()
        {
            var transactions = new List<TransactionDetailsQueryModel>
            {
                new TransactionDetailsQueryModel
                {
                    Amount = 1m,
                    InstrumentPriceAtRangeStart = 271.83m,
                    InstrumentPriceAtRangeEnd = 314.16m,
                    Price = 299.99m,
                    Time = DateTime.Parse("2019-12-01 06:29")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = 3m,
                    InstrumentPriceAtRangeStart = 271.83m,
                    InstrumentPriceAtRangeEnd = 314.16m,
                    Price = 304.66m,
                    Time = DateTime.Parse("2019-12-05 07:37")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -4m,
                    InstrumentPriceAtRangeStart = 271.83m,
                    InstrumentPriceAtRangeEnd = 314.16m,
                    Price = 312.54m,
                    Time = DateTime.Parse("2019-12-14 19:59")
                }
            };

            var start = DateTime.Parse("2019-01-01");
            var end = DateTime.Parse("2020-01-01");

            var expected = 0.124m;

            return new object[] { transactions, start, end, expected };
        }

        private static object[] GenerateTestData_MultipleTransactions_NegativeIrr()
        {
            var transactions = new List<TransactionDetailsQueryModel>
            {
                new TransactionDetailsQueryModel
                {
                    Amount = 4m,
                    InstrumentPriceAtRangeStart = 931.12m,
                    InstrumentPriceAtRangeEnd = 914.55m,
                    Price = 955.16m,
                    Time = DateTime.Parse("2015-08-16 14:22")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -2m,
                    InstrumentPriceAtRangeStart = 931.12m,
                    InstrumentPriceAtRangeEnd = 914.55m,
                    Price = 934.59m,
                    Time = DateTime.Parse("2016-03-29 18:46")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -1m,
                    InstrumentPriceAtRangeStart = 931.12m,
                    InstrumentPriceAtRangeEnd = 914.55m,
                    Price = 867.54m,
                    Time = DateTime.Parse("2017-09-11 01:23")
                }
            };

            var start = DateTime.Parse("2015-01-01");
            var end = DateTime.Parse("2018-01-01");

            var expected = -0.075m;

            return new object[] { transactions, start, end, expected };
        }

        private static object[] GenerateTestData_MultipleTransactions_Complex()
        {
            var transactions = new List<TransactionDetailsQueryModel>
            {
                new TransactionDetailsQueryModel
                {
                    Amount = 1000,
                    InstrumentPriceAtRangeStart = 100m,
                    InstrumentPriceAtRangeEnd = 400m,
                    Price = 100m,
                    Time = DateTime.Parse("2004-01-01")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = 500,
                    InstrumentPriceAtRangeStart = 100m,
                    InstrumentPriceAtRangeEnd = 400m,
                    Price = 116.7m,
                    Time = DateTime.Parse("2006-12-01")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -300,
                    InstrumentPriceAtRangeStart = 100m,
                    InstrumentPriceAtRangeEnd = 400m,
                    Price = 274.59m,
                    Time = DateTime.Parse("2014-01-01")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = 1000,
                    InstrumentPriceAtRangeStart = 100m,
                    InstrumentPriceAtRangeEnd = 400m,
                    Price = 314.59m,
                    Time = DateTime.Parse("2018-07-15")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = -1250,
                    InstrumentPriceAtRangeStart = 100m,
                    InstrumentPriceAtRangeEnd = 400m,
                    Price = 305.26m,
                    Time = DateTime.Parse("2019-04-12")
                },
                new TransactionDetailsQueryModel
                {
                    Amount = 2000,
                    InstrumentPriceAtRangeStart = 100m,
                    InstrumentPriceAtRangeEnd = 400m,
                    Price = 341.87m,
                    Time = DateTime.Parse("2020-11-11")
                },
                new TransactionDetailsQueryModel()
                {
                    Amount = -1561,
                    InstrumentPriceAtRangeStart = 100m,
                    InstrumentPriceAtRangeEnd = 400m,
                    Price = 387.33m,
                    Time = DateTime.Parse("2021-05-16")
                }
            };

            var start = DateTime.Parse("2000-01-01");
            var end = DateTime.Parse("2022-01-01");

            var expected = 3.293m;

            return new object[] { transactions, start, end, expected };
        }
    }
}
