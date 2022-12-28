using System;

namespace PortEval.Domain.Models.ValueObjects
{
    public class TrackingInformation
    {
        public DateTime StartTime { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public DateTime TrackedSince { get; private set; }

        public TrackingInformation(DateTime startTime, DateTime trackedSince)
        {
            StartTime = startTime;
            TrackedSince = trackedSince;
        }

        public void Update(DateTime updateTime)
        {
            LastUpdate = updateTime;
        }
    }
}
