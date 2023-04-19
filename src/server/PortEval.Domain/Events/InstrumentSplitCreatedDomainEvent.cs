using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events;

/// <summary>
///     A domain event representing the creation of an instrument split.
/// </summary>
public class InstrumentSplitCreatedDomainEvent : IDomainEvent
{
    /// <summary>
    ///     The created instrument split.
    /// </summary>
    public InstrumentSplit InstrumentSplit { get; private set; }

    /// <summary>
    ///     Initializes the domain event.
    /// </summary>
    /// <param name="instrumentSplit">The created instrument split.</param>
    public InstrumentSplitCreatedDomainEvent(InstrumentSplit instrumentSplit)
    {
        InstrumentSplit = instrumentSplit;
    }
}