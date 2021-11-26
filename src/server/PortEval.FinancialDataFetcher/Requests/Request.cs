namespace PortEval.FinancialDataFetcher.Requests
{
    /// <summary>
    /// A base class for all price fetcher's request types.
    /// </summary>
    public abstract class Request
    {
        public string Symbol { get; set; }
    }
}
