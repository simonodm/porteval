using PortEval.DataFetcher.Interfaces;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests
{
    public class InstrumentDataRequest : IRequest
    {
        public string Symbol { get; set; }
        public string CurrencyCode { get; set; }
    }
}
