using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.BackgroundJobs
{
    public interface IInitialPriceFetchJob
    {
        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve prices for.</param>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        Task Run(int instrumentId);
    }
}