namespace PortEval.Infrastructure.Queries.Models;

/// <summary>
///     An internal helper model for matching a financial entity's name and the queried chart line's name.
/// </summary>
public class ChartLineNameModel
{
    public string PortfolioName { get; set; }
    public string PositionName { get; set; }
    public string InstrumentName { get; set; }
}