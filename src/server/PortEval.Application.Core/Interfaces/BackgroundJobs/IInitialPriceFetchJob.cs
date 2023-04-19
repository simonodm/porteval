using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs;

/// <summary>
///     A job responsible for downloading a complete price history for a newly created instrument.
/// </summary>
public interface IInitialPriceFetchJob
{
    /// <summary>
    ///     Executes the job.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to retrieve prices for.</param>
    /// <returns>A task representing the asynchronous price download and import operations.</returns>
    public Task RunAsync(int instrumentId);
}