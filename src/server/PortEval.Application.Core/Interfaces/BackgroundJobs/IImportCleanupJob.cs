using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs;

/// <summary>
///     A job responsible for removing bulk import files and entries older than 24 hours.
/// </summary>
public interface IImportCleanupJob
{
    /// <summary>
    ///     Executes the job.
    /// </summary>
    /// <returns>A task representing the asynchronous cleanup operation.</returns>
    public Task RunAsync();
}