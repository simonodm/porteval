using PortEval.FinancialDataFetcher.Interfaces.APIs;
using PortEval.FinancialDataFetcher.Responses;

namespace PortEval.Tests.Unit.FinancialDataFetcherTests.HelperTypes
{
    public interface ITestFinancialApi : IFinancialApi<TestRequest, Response<bool>>
    {
    }
}
