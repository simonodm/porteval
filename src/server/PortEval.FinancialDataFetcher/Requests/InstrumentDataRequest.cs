using PortEval.FinancialDataFetcher.Interfaces;

namespace PortEval.FinancialDataFetcher.Requests
{
    public class InstrumentDataRequest : IRequest
    {
        public string Symbol { get; set; }
        public string CurrencyCode { get; set; }
    }
}
