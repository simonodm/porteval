﻿using System;

namespace PortEval.BackgroundJobs.Helpers
{
    /// <summary>
    /// Represents a range between two times and an interval.
    /// </summary>
    public struct TimeRange
    {
        public DateTime From;
        public DateTime To;
        public TimeSpan Interval;
    }
}