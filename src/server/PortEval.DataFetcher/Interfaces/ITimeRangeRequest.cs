using System;

namespace PortEval.DataFetcher.Interfaces;

/// <summary>
///     Represents a request for data in a specific date/time range.
/// </summary>
public interface ITimeRangeRequest : IRequest
{
    /// <summary>
    ///     Date range start.
    /// </summary>
    public DateTime From { get; set; }

    /// <summary>
    ///     Date range end.
    /// </summary>
    public DateTime To { get; set; }
}