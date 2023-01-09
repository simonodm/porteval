using PortEval.Domain.Events;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System;

namespace PortEval.Domain.Models.Entities
{
    public class InstrumentSplit : VersionedEntity, IAggregateRoot
    {
        public int Id { get; private set; }
        public int InstrumentId { get; private set; }
        public DateTime Time { get; private set; }
        public InstrumentSplitProcessingStatus ProcessingStatus { get; private set; }
        public SplitRatio SplitRatio { get; private set; }

        internal InstrumentSplit()
        {
        }

        internal InstrumentSplit(int id, int instrumentId, DateTime time, SplitRatio splitRatio) : this(instrumentId,
            time, splitRatio)
        {
            Id = id;
        }

        internal InstrumentSplit(int instrumentId, DateTime time, SplitRatio splitRatio)
        {
            InstrumentId = instrumentId;
            Time = time;
            SplitRatio = splitRatio;
            ProcessingStatus = InstrumentSplitProcessingStatus.NotProcessed;
        }

        public static InstrumentSplit Create(Instrument instrument, DateTime time, SplitRatio splitRatio)
        {
            var split = new InstrumentSplit(instrument.Id, time, splitRatio);
            split.AddDomainEvent(new InstrumentSplitCreatedDomainEvent(split));

            return split;
        }

        public void MarkAsProcessed()
        {
            ProcessingStatus = InstrumentSplitProcessingStatus.Processed;
        }

        public void Rollback()
        {
            if (ProcessingStatus == InstrumentSplitProcessingStatus.NotProcessed)
            {
                MarkAsRolledBack();
            }
            else if (ProcessingStatus == InstrumentSplitProcessingStatus.Processed)
            {
                ProcessingStatus = InstrumentSplitProcessingStatus.RollbackRequested;
                AddDomainEvent(new InstrumentSplitRollbackRequestedDomainEvent(this));
            }
        }

        public void MarkAsRolledBack()
        {
            ProcessingStatus = InstrumentSplitProcessingStatus.RolledBack;
        }
    }
}
