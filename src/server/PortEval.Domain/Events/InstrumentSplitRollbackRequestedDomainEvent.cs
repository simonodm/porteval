using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events
{
    public class InstrumentSplitRollbackRequestedDomainEvent : IDomainEvent
    {
        public InstrumentSplit InstrumentSplit { get; init; }

        public InstrumentSplitRollbackRequestedDomainEvent(InstrumentSplit instrumentSplit)
        {
            InstrumentSplit = instrumentSplit;
        }
    }
}
