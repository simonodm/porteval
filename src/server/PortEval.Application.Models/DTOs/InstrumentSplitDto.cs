using System;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Models.DTOs
{
    public class InstrumentSplitDto
    {
        public int Id { get; set; }
        public int InstrumentId { get; set; }
        public DateTime Time { get; set; }
        public int SplitRatioDenominator { get; set; }
        public int SplitRatioNumerator { get; set; }
        public InstrumentSplitProcessingStatus Status { get; set; }
    }
}
