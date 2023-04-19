namespace PortEval.Domain.Models.Enums;

/// <summary>
///     Represents the status of the automatic tracking of an instrument.
/// </summary>
public enum InstrumentTrackingStatus
{
    Created,
    SearchingForPrices,
    Tracked,
    Untracked
}