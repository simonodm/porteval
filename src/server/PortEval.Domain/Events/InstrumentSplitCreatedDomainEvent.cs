using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events
{
    public class InstrumentSplitCreatedDomainEvent : IDomainEvent
    {
        public InstrumentSplit InstrumentSplit { get; private set; }

        public InstrumentSplitCreatedDomainEvent(InstrumentSplit instrumentSplit)
        {
            InstrumentSplit = instrumentSplit;
        }
    }
}
