using System.Collections.Generic;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;

namespace PortEval.FinancialDataFetcher.Interfaces.APIs
{
    public interface IInstrumentSplitFinancialApi : IFinancialApi<InstrumentSplitsRequest, Response<IEnumerable<InstrumentSplitData>>>
    {
    }
}
