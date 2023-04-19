using System;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents a single chart data point.")]
public class EntityChartPointDto
{
    [SwaggerSchema("Data point time.", ReadOnly = true)]
    public DateTime Time { get; }

    [SwaggerSchema("Data point value.", ReadOnly = true)]
    public decimal Value { get; private set; }

    public EntityChartPointDto(DateTime time, decimal price)
    {
        Time = time;
        Value = price;
    }

    public EntityChartPointDto ChangeValue(decimal newPrice)
    {
        return new EntityChartPointDto(Time, newPrice);
    }

    public static EntityChartPointDto FromInstrumentPrice(InstrumentPriceDto price)
    {
        return new EntityChartPointDto(price.Time, price.Price);
    }

    public static EntityChartPointDto FromValue(EntityValueDto value)
    {
        return new EntityChartPointDto(value.Time, value.Value);
    }

    public static EntityChartPointDto FromProfit(EntityProfitDto value)
    {
        return new EntityChartPointDto(value.To, value.Profit);
    }

    public static EntityChartPointDto FromPerformance(EntityPerformanceDto value)
    {
        return new EntityChartPointDto(value.To, value.Performance);
    }
}