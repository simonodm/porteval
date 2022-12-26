using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.BackgroundJobs
{
    public interface IInstrumentPriceCleanupJob
    {
        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        Task Run();
    }
}