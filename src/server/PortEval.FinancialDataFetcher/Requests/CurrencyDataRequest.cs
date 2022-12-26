using PortEval.FinancialDataFetcher.Interfaces;

namespace PortEval.FinancialDataFetcher.Requests
{
    public class CurrencyDataRequest : IRequest
    {
        public string CurrencyCode { get; set; }
    }
}
