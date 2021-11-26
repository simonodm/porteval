using System;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.Models
{
    /// <summary>
    /// Represents exchange rates from a single currency to multiple currencies at a single point in time.
    /// </summary>
    public class ExchangeRates
    {
        public string Currency { get; set; }
        public DateTime Time { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
