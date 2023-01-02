using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events
{
    public class InstrumentCreatedDomainEvent : IDomainEvent
    {
        public Instrument Instrument { get; init; }

        public InstrumentCreatedDomainEvent(Instrument instrument)
        {
            Instrument = instrument;
        }
    }
}
