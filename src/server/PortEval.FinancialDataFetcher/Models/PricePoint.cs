using System;

namespace PortEval.FinancialDataFetcher.Models
{
    /// <summary>
    /// Represents a single instrument price at a single point in time.
    /// </summary>
    public class PricePoint
    {
        public string Symbol { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }
}
