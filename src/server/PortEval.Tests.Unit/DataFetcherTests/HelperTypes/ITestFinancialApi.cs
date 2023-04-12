using PortEval.DataFetcher;
using PortEval.DataFetcher.Responses;
using System.Threading.Tasks;

namespace PortEval.Tests.Unit.DataFetcherTests.HelperTypes
{
    public abstract class TestFinancialApi : DataSource
    {
        [RequestProcessor(typeof(TestRequest), typeof(bool))]
        public abstract Task<Response<bool>> Process(TestRequest request);
    }
}
