using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.ValueObjects;

/// <summary>
///     Represents the chart type and type-specific configuration.
/// </summary>
public class ChartTypeSettings : ValueObject
{
    /// <summary>
    ///     The type of the chart.
    /// </summary>
    public ChartType Type { get; private set; }

    /// <summary>
    ///     The aggregation frequency of the aggregated chart.
    ///     This field is only populated if <see cref="Type" /> is <see cref="ChartType.AggregatedProfit" /> or
    ///     <see cref="ChartType.AggregatedPerformance" />.
    /// </summary>
    public AggregationFrequency? Frequency { get; private set; }

    /// <summary>
    ///     The 3-letter currency code of a monetary chart.
    ///     This field is only populated if <see cref="Type" /> is <see cref="ChartType.Price" />,
    ///     <see cref="ChartType.Profit" />, or <see cref="ChartType.AggregatedProfit" />.
    /// </summary>
    public string CurrencyCode { get; private set; }

    private ChartTypeSettings()
    {
    }

    /// <summary>
    ///     A factory method for creating a price chart configuration.
    /// </summary>
    /// <param name="currencyCode">The 3-letter currency code of the price chart.</param>
    /// <returns>The newly created price type configuration.</returns>
    public static ChartTypeSettings PriceChart(string currencyCode)
    {
        return new ChartTypeSettings
        {
            Type = ChartType.Price,
            CurrencyCode = currencyCode
        };
    }

    /// <summary>
    ///     A factory method for creating a profit chart configuration.
    /// </summary>
    /// <param name="currencyCode">The 3-letter currency code of the profit chart.</param>
    /// <returns>The newly created profit type configuration.</returns>
    public static ChartTypeSettings ProfitChart(string currencyCode)
    {
        return new ChartTypeSettings
        {
            Type = ChartType.Profit,
            CurrencyCode = currencyCode
        };
    }

    /// <summary>
    ///     A factory method for creating a performance chart configuration.
    /// </summary>
    /// <returns>The newly created performance type configuration.</returns>
    public static ChartTypeSettings PerformanceChart()
    {
        return new ChartTypeSettings
        {
            Type = ChartType.Performance
        };
    }

    /// <summary>
    ///     A factory method for creating an aggregated profit chart configuration.
    /// </summary>
    /// <param name="currencyCode">The 3-letter currency code of the aggregated profit chart.</param>
    /// <param name="frequency">The aggregation frequency of the chart.</param>
    /// <returns>The newly created aggregated profit type configuration.</returns>
    public static ChartTypeSettings AggregatedProfitChart(string currencyCode, AggregationFrequency frequency)
    {
        return new ChartTypeSettings
        {
            Type = ChartType.AggregatedProfit,
            CurrencyCode = currencyCode,
            Frequency = frequency
        };
    }

    /// <summary>
    ///     A factory method for creating an aggregated performance chart configuration.
    /// </summary>
    /// <param name="frequency">The 3-letter currency code of the aggregated performance chart.</param>
    /// <returns>The newly created aggregated performance type configuration.</returns>
    public static ChartTypeSettings AggregatedPerformanceChart(AggregationFrequency frequency)
    {
        return new ChartTypeSettings
        {
            Type = ChartType.AggregatedPerformance,
            Frequency = frequency
        };
    }
}