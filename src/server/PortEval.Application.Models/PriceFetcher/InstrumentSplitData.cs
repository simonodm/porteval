using System;

namespace PortEval.Application.Models.PriceFetcher
{
    public class InstrumentSplitData
    {
        public DateTime Time { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }
    }
}
