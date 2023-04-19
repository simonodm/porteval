namespace PortEval.Domain.Models.Enums;

/// <summary>
///     Represents the processing status of a bulk data import.
/// </summary>
public enum ImportStatus
{
    Finished,
    InProgress,
    Error,
    Pending
}