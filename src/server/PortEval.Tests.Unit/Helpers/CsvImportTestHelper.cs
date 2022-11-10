using System.Collections.Generic;
using PortEval.Domain.Models.Enums;

namespace PortEval.Tests.Helpers
{
    internal static class CsvImportTestHelper
    {
        public static Dictionary<TemplateType, string[]> TemplateExpectedHeaders =
            new()
            {
                {
                    TemplateType.Instruments,
                    new[] { "Instrument ID", "Symbol", "Name", "Exchange", "Type", "Currency", "Note" }
                },
                {
                    TemplateType.Portfolios,
                    new[] { "Portfolio ID", "Name", "Currency", "Note" }
                },
                {
                    TemplateType.Positions,
                    new[] { "Position ID", "Instrument ID", "Portfolio ID", "Note", "Time", "Amount", "Price" }
                },
                {
                    TemplateType.Transactions,
                    new[] { "Transaction ID", "Position ID", "Price", "Amount", "Time" }
                },
                {
                    TemplateType.Prices,
                    new[] { "Price ID", "Instrument ID", "Price", "Time" }
                }
            };

        public static Dictionary<TemplateType, string[]> TemplateTestData = new()
        {
            {
                TemplateType.Instruments,
                new[] { "1", "AAPL", "Apple Inc.", "NASDAQ", "Stock", "USD", "Test Note" }
            },
            {
                TemplateType.Portfolios,
                new[] { "2", "US stocks", "USD", "Test Note Portfolio" }
            },
            {
                TemplateType.Positions,
                new[] { "3", "1", "1", "Test Note Position", "2021/06/12 13:35", "1", "120" }
            },
            {
                TemplateType.Transactions,
                new[] { "4", "2", "5", "2022/01/05 12:00" }
            },
            {
                TemplateType.Prices,
                new[] { "5", "3", "124.41", "2022/10/22 00:00" }
            }
        };
    }
}