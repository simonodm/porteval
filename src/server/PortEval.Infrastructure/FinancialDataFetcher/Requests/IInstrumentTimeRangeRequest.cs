namespace PortEval.Infrastructure.FinancialDataFetcher.Requests;

internal interface IInstrumentTimeRangeRequest : ITimeRangeRequest
{
    public string Symbol { get; set; }
    public string CurrencyCode { get; set; }
}