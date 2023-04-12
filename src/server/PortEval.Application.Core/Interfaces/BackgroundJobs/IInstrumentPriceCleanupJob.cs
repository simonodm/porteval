using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs
{
    /// <summary>
    /// A job responsible for deleting overabundant instrument prices to reduce storage size.
    /// </summary>
    public interface IInstrumentPriceCleanupJob
    {
        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <returns>A task representing the asynchronous cleanup operation.</returns>
        public Task RunAsync();
    }
}