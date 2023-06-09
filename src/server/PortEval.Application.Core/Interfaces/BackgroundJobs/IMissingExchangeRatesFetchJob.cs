﻿using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.BackgroundJobs;

/// <summary>
///     A job responsible for downloading missing exchange rates of the application's default currency.
/// </summary>
public interface IMissingExchangeRatesFetchJob
{
    /// <summary>
    ///     Executes the job.
    /// </summary>
    /// <returns>A task representing the asynchronous exchange rate download and import operations.</returns>
    public Task RunAsync();
}