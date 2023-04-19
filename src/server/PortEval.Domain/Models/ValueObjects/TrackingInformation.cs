using System;

namespace PortEval.Domain.Models.ValueObjects;

/// <summary>
///     Represents information about an entity which is automatically tracked and maintained by the application.
/// </summary>
public class TrackingInformation
{
    /// <summary>
    ///     The time of the first available data for the tracked entity.
    /// </summary>
    public DateTime StartTime { get; private set; }

    /// <summary>
    ///     The time of the last data update for the tracked entity.
    /// </summary>
    public DateTime LastUpdate { get; private set; }

    /// <summary>
    ///     The time when the entity started being tracked.
    /// </summary>
    public DateTime TrackedSince { get; private set; }

    /// <summary>
    ///     Initializes the tracking information.
    /// </summary>
    /// <param name="startTime">The time of the first available data for the tracked entity.</param>
    /// <param name="trackedSince">The time when the entity started being tracked.</param>
    public TrackingInformation(DateTime startTime, DateTime trackedSince)
    {
        StartTime = startTime;
        TrackedSince = trackedSince;
    }

    /// <summary>
    ///     Updates the time of the last data update for the tracked entity.
    /// </summary>
    /// <param name="updateTime">The new time of the last data update.</param>
    public void Update(DateTime updateTime)
    {
        LastUpdate = updateTime;
    }
}