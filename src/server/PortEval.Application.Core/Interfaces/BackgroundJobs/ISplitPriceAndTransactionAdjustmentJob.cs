using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs
{
    /// <summary>
    /// A job responsible for adjusting instrument prices and transactions after a change in instrument splits.
    /// </summary>
    public interface ISplitPriceAndTransactionAdjustmentJob
    {
        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <returns>A task representing the asynchronous adjustment operation.</returns>
        public Task RunAsync();
    }
}
