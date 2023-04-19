using System;
using PortEval.DataFetcher.Interfaces;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests;

/// <summary>
///     Represents a request for data in some date/time range.
/// </summary>
public interface ITimeRangeRequest : IRequest
{
    /// <summary>
    ///     The start date/time of the range to retrieve data in.
    /// </summary>
    public DateTime From { get; set; }

    /// <summary>
    ///     The end date/time of the range to retrieve data in.
    /// </summary>
    public DateTime To { get; set; }
}