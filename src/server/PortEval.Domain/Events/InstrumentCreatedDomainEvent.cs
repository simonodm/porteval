using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events;

/// <summary>
///     A domain event representing the creation of a new instrument.
/// </summary>
public class InstrumentCreatedDomainEvent : IDomainEvent
{
    /// <summary>
    ///     The created instrument.
    /// </summary>
    public Instrument Instrument { get; init; }

    /// <summary>
    ///     Initializes the domain event.
    /// </summary>
    /// <param name="instrument">The created instrument.</param>
    public InstrumentCreatedDomainEvent(Instrument instrument)
    {
        Instrument = instrument;
    }
}