using PortEval.Application.Features.Common;
using PortEval.Application.Models.DTOs;
using System;

namespace PortEval.Application.Features.Interfaces.Calculators
{
    public interface IPositionStatisticsCalculator
    {
        PositionStatisticsDto CalculateStatistics(
            PositionPriceListData positionPriceData, DateTime time);
    }
}