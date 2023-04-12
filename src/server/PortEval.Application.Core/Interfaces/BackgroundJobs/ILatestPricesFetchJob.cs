using Hangfire;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs
{
    /// <summary>
    /// A job responsible for downloading the latest prices of created instruments.
    /// </summary>
    public interface ILatestPricesFetchJob
    {
        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous price download and import operations.</returns>
        [AutomaticRetry(Attempts = 0)]
        public Task RunAsync();
    }
}