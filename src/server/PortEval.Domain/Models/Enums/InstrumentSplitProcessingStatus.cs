namespace PortEval.Domain.Models.Enums;

/// <summary>
///     Represents the processing status of an instrument split.
/// </summary>
public enum InstrumentSplitProcessingStatus
{
    NotProcessed,
    Processed,
    RollbackRequested,
    RolledBack
}