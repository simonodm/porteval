using System;

namespace PortEval.Application.Models.FinancialDataFetcher;

/// <summary>
///     Contains information about a retrieved instrument split.
/// </summary>
public class InstrumentSplitData
{
    /// <summary>
    ///     Time of the split.
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    ///     Split's factor numerator.
    /// </summary>
    public int Numerator { get; set; }

    /// <summary>
    ///     Split's factor denominator.
    /// </summary>
    public int Denominator { get; set; }
}