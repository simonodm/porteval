using Hangfire;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs
{
    /// <summary>
    /// A job responsible for downloading the latest exchange rates of the application's default currency.
    /// </summary>
    public interface ILatestExchangeRatesFetchJob
    {
        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <returns>A task representing the asynchronous exchange rate download and import operations.</returns>
        [AutomaticRetry(Attempts = 0)]
        public Task RunAsync();
    }
}