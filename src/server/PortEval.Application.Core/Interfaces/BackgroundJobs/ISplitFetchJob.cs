using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs
{
    /// <summary>
    /// A job responsible for downloading missing instrument splits.
    /// </summary>
    public interface ISplitFetchJob
    {
        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <returns>A task responsible for the asynchronous split download and import operations.</returns>
        public Task RunAsync();
    }
}
