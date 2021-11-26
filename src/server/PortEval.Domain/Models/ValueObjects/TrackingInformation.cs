using System;

namespace PortEval.Domain.Models.ValueObjects
{
    public class TrackingInformation
    {
        public DateTime StartTime { get; private set; }

        public TrackingInformation(DateTime startTime)
        {
            StartTime = startTime;
        }
    }
}
