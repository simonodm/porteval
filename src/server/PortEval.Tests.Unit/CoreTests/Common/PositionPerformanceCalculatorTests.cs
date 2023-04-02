using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Common.Calculators;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Common
{
    public class PositionPerformanceCalculatorTests
    {
        [Theory]
        [MemberData(nameof(GenerateTestData))]
        public void CalculateIrr_ReturnsCorrectRateOfReturn(IEnumerable<PositionPriceRangeData> positionsPriceData, DateTime start, DateTime end, decimal expectedIrr)
        {
            var calculator = new PositionPerformanceCalculator();

            var result = calculator.CalculatePerformance(positionsPriceData, start, end);

            Assert.InRange(result, expectedIrr - 0.01m, expectedIrr + 0.01m);
        }

        [Fact]
        public void CalculateIrr_ReturnsZeroIrr_WhenNoTransactionsAreProvided()
        {
            var calculator = new PositionPerformanceCalculator();

            var result = calculator.CalculatePerformance(Enumerable.Empty<PositionPriceRangeData>(),
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
            var start = DateTime.Parse("2021-12-25");
            var end = DateTime.Parse("2022-03-01");

            var positionsPriceData = new List<PositionPriceRangeData>
            {
                new PositionPriceRangeData
                {
                    Transactions = new[]
                    {
                        new TransactionDto
                        {
                            Amount = 1m,
                            Price = 113m,
                            Time = DateTime.Parse("2022-02-01 14:00")
                        }
                    },
                    PriceAtRangeStart = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 100m,
                        Time = start
                    },
                    PriceAtRangeEnd = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 125m,
                        Time = end
                    }
                }
            };

            var expected = 0.106m;

            return new object[] { positionsPriceData, start, end, expected };
        }

        private static object[] GenerateTestData_WithSaleTransaction_PositiveIrr()
        {
            var start = DateTime.Parse("2020-05-15");
            var end = DateTime.Parse("2020-07-15");

            var positionsPriceData = new List<PositionPriceRangeData>
            {
                new PositionPriceRangeData
                {
                    Transactions = new[]
                    {
                        new TransactionDto
                        {
                            Amount = 1m,
                            Price = 92.14m,
                            Time = DateTime.Parse("2020-06-16 15:35")
                        },
                        new TransactionDto
                        {
                            Amount = -1m,
                            Price = 99.13m,
                            Time = DateTime.Parse("2020-07-01 16:24")
                        }
                    },
                    PriceAtRangeStart = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 97.14m,
                        Time = start
                    },
                    PriceAtRangeEnd = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 103.12m,
                        Time = end
                    }
                }
            };

            var expected = 0.157m;

            return new object[] { positionsPriceData, start, end, expected };
        }

        private static object[] GenerateTestData_WithSaleTransaction_NegativeIrr()
        {
            var start = DateTime.Parse("2020-05-15");
            var end = DateTime.Parse("2020-07-15");

            var positionsPriceData = new List<PositionPriceRangeData>
            {
                new PositionPriceRangeData
                {
                    Transactions = new[]
                    {
                        new TransactionDto
                        {
                            Amount = 1m,
                            Price = 92.14m,
                            Time = DateTime.Parse("2020-06-16 15:35")
                        },
                        new TransactionDto
                        {
                            Amount = -1m,
                            Price = 90.55m,
                            Time = DateTime.Parse("2020-07-01 16:24")
                        }
                    },
                    PriceAtRangeStart = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 97.14m,
                        Time = start
                    },
                    PriceAtRangeEnd = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 103.12m,
                        Time = end
                    }
                }
            };

            var expected = -0.034m;

            return new object[] { positionsPriceData, start, end, expected };
        }

        private static object[] GenerateTestData_MultipleTransactions_PositiveIrr()
        {
            var start = DateTime.Parse("2019-01-01");
            var end = DateTime.Parse("2020-01-01");

            var positionsPriceData = new List<PositionPriceRangeData>
            {
                new PositionPriceRangeData
                {
                    Transactions = new[]
                    {
                        new TransactionDto
                        {
                            Amount = 1m,
                            Price = 299.99m,
                            Time = DateTime.Parse("2019-12-01 06:29")
                        },
                        new TransactionDto
                        {
                            Amount = 3m,
                            Price = 304.66m,
                            Time = DateTime.Parse("2019-12-05 07:37")
                        },
                        new TransactionDto
                        {
                            Amount = -4m,
                            Price = 312.54m,
                            Time = DateTime.Parse("2019-12-14 19:59")
                        }
                    },
                    PriceAtRangeStart = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 271.83m,
                        Time = start
                    },
                    PriceAtRangeEnd = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 314.16m,
                        Time = end
                    }
                }
            };

            var expected = 0.124m;

            return new object[] { positionsPriceData, start, end, expected };
        }

        private static object[] GenerateTestData_MultipleTransactions_NegativeIrr()
        {
            var start = DateTime.Parse("2015-01-01");
            var end = DateTime.Parse("2018-01-01");

            var positionsPriceData = new[]
            {
                new PositionPriceRangeData
                {
                    Transactions = new List<TransactionDto>
                    {
                        new TransactionDto
                        {
                            Amount = 4m,
                            Price = 955.16m,
                            Time = DateTime.Parse("2015-08-16 14:22")
                        },
                        new TransactionDto
                        {
                            Amount = -2m,
                            Price = 934.59m,
                            Time = DateTime.Parse("2016-03-29 18:46")
                        },
                        new TransactionDto
                        {
                            Amount = -1m,
                            Price = 867.54m,
                            Time = DateTime.Parse("2017-09-11 01:23")
                        }
                    },
                    PriceAtRangeStart = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 931.12m,
                        Time = start
                    },
                    PriceAtRangeEnd = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 914.55m,
                        Time = end
                    }
                }
            };



            var expected = -0.075m;

            return new object[] { positionsPriceData, start, end, expected };
        }

        private static object[] GenerateTestData_MultipleTransactions_Complex()
        {
            var start = DateTime.Parse("2000-01-01");
            var end = DateTime.Parse("2022-01-01");

            var positionsPriceData = new[]
            {
                new PositionPriceRangeData
                {
                    Transactions = new List<TransactionDto>
                    {
                        new TransactionDto
                        {
                            Amount = 1000,
                            Price = 100m,
                            Time = DateTime.Parse("2004-01-01")
                        },
                        new TransactionDto
                        {
                            Amount = 500,
                            Price = 116.7m,
                            Time = DateTime.Parse("2006-12-01")
                        },
                        new TransactionDto
                        {
                            Amount = -300,
                            Price = 274.59m,
                            Time = DateTime.Parse("2014-01-01")
                        },
                        new TransactionDto
                        {
                            Amount = 1000,
                            Price = 314.59m,
                            Time = DateTime.Parse("2018-07-15")
                        },
                        new TransactionDto
                        {
                            Amount = -1250,
                            Price = 305.26m,
                            Time = DateTime.Parse("2019-04-12")
                        },
                        new TransactionDto
                        {
                            Amount = 2000,
                            Price = 341.87m,
                            Time = DateTime.Parse("2020-11-11")
                        },
                        new TransactionDto
                        {
                            Amount = -1561,
                            Price = 387.33m,
                            Time = DateTime.Parse("2021-05-16")
                        }
                    },
                    PriceAtRangeStart = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 100m,
                        Time = start
                    },
                    PriceAtRangeEnd = new InstrumentPriceDto
                    {
                        InstrumentId = 1,
                        Price = 400m,
                        Time = end
                    }
                }
            };

            var expected = 3.293m;

            return new object[] { positionsPriceData, start, end, expected };
        }
    }
}
