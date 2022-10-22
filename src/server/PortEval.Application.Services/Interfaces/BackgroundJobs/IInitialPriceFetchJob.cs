using Hangfire;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.BackgroundJobs
{
    public interface IInitialPriceFetchJob
    {
        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve prices for.</param>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        [AutomaticRetry(Attempts = 0)]
        Task Run(int instrumentId);
    }
}