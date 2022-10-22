using System;

namespace PortEval.Domain.Models.ValueObjects
{
    public class TrackingInformation
    {
        public DateTime StartTime { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public TrackingInformation(DateTime startTime)
        {
            StartTime = startTime;
        }

        public void Update(DateTime updateTime)
        {
            LastUpdate = updateTime;
        }
    }
}
