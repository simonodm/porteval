using PortEval.DataFetcher.Interfaces;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests
{
    public class CurrencyDataRequest : IRequest
    {
        public string CurrencyCode { get; set; }
    }
}
