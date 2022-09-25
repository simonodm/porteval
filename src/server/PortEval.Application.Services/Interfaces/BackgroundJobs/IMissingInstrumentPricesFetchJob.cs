using Hangfire;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.BackgroundJobs
{
    public interface IMissingInstrumentPricesFetchJob
    {
        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        [AutomaticRetry(Attempts = 0)]
        Task Run();
    }
}