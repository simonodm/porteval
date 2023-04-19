using System;
using PortEval.Domain.Events;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents a split of an instrument.
/// </summary>
public class InstrumentSplit : VersionedEntity, IAggregateRoot
{
    /// <summary>
    ///     ID of the split.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     ID of the parent instrument.
    /// </summary>
    public int InstrumentId { get; private set; }

    /// <summary>
    ///     Time of the split.
    /// </summary>
    public DateTime Time { get; private set; }

    /// <summary>
    ///     The processing status of the split in the context of the application.
    /// </summary>
    public InstrumentSplitProcessingStatus ProcessingStatus { get; private set; }

    /// <summary>
    ///     Represents the split ratio.
    /// </summary>
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

    /// <summary>
    ///     A factory method for creating instrument splits.
    /// </summary>
    /// <param name="instrument">A reference to the parent instrument.</param>
    /// <param name="time">The time of the split.</param>
    /// <param name="splitRatio">The split ratio.</param>
    /// <returns>The newly created instrument split.</returns>
    public static InstrumentSplit Create(Instrument instrument, DateTime time, SplitRatio splitRatio)
    {
        var split = new InstrumentSplit(instrument.Id, time, splitRatio);
        split.AddDomainEvent(new InstrumentSplitCreatedDomainEvent(split));

        return split;
    }

    /// <summary>
    ///     Marks this instrument split as processed.
    /// </summary>
    public void MarkAsProcessed()
    {
        ProcessingStatus = InstrumentSplitProcessingStatus.Processed;
    }

    /// <summary>
    ///     Rolls back the split.
    /// </summary>
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

    /// <summary>
    ///     Marks the split as successfully rolled back.
    /// </summary>
    public void MarkAsRolledBack()
    {
        ProcessingStatus = InstrumentSplitProcessingStatus.RolledBack;
    }
}