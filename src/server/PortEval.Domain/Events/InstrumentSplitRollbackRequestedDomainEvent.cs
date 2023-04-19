using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events;

/// <summary>
///     A domain event representing the rollback of a processed instrument split.
/// </summary>
public class InstrumentSplitRollbackRequestedDomainEvent : IDomainEvent
{
    /// <summary>
    ///     The instrument split being rolled back.
    /// </summary>
    public InstrumentSplit InstrumentSplit { get; init; }

    /// <summary>
    ///     Initializes the domain event.
    /// </summary>
    /// <param name="instrumentSplit">The instrument split being rolled back.</param>
    public InstrumentSplitRollbackRequestedDomainEvent(InstrumentSplit instrumentSplit)
    {
        InstrumentSplit = instrumentSplit;
    }
}