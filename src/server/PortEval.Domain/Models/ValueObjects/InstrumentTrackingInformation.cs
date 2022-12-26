using PortEval.Domain.Models.Enums;
using System;

namespace PortEval.Domain.Models.ValueObjects
{
    public class InstrumentTrackingInformation : TrackingInformation
    {
        public InstrumentTrackingStatus TrackingStatus { get; private set; }

        public InstrumentTrackingInformation(DateTime startTime, InstrumentTrackingStatus trackingStatus) : base(startTime)
        {
            TrackingStatus = trackingStatus;
        }

        public void SetTrackingStatus(InstrumentTrackingStatus trackingStatus)
        {
            TrackingStatus = trackingStatus;
        }
    }
}
