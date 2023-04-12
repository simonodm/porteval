using Hangfire;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs
{
    /// <summary>
    /// A job responsible for downloading missing instrument prices.
    /// </summary>
    public interface IMissingInstrumentPricesFetchJob
    {
        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <returns>A task representing the asynchronous price download and import operations.</returns>
        [AutomaticRetry(Attempts = 0)]
        public Task RunAsync();
    }
}