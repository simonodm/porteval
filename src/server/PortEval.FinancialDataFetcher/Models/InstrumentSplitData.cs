using System;

namespace PortEval.FinancialDataFetcher.Models
{
    public class InstrumentSplitData
    {
        public DateTime Time { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }
    }
}
