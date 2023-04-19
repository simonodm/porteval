namespace PortEval.Application.Models.QueryParams;

/// <summary>
///     Represents transaction filters enabling filtering by a financial entity on which the transaction was executed.
/// </summary>
public class TransactionFilters
{
    /// <summary>
    ///     ID of the portfolio to filter transactions by.
    /// </summary>
    public int? PortfolioId { get; set; }

    /// <summary>
    ///     ID of the position to filter transactions by.
    /// </summary>
    public int? PositionId { get; set; }

    /// <summary>
    ///     ID of the instrument to filter transactions by.
    /// </summary>
    public int? InstrumentId { get; set; }

    /// <summary>
    ///     Creates an instance of <see cref="TransactionFilters" /> filtering by the specified portfolio ID.
    /// </summary>
    /// <param name="portfolioId">ID of the portfolio to filter transactions by.</param>
    /// <returns>A <see cref="TransactionFilters" /> instance filtering by the specified portfolio ID.</returns>
    public static TransactionFilters FromPortfolioId(int portfolioId)
    {
        return new TransactionFilters
        {
            PortfolioId = portfolioId
        };
    }

    /// <summary>
    ///     Creates an instance of <see cref="TransactionFilters" /> filtering by the specified position ID.
    /// </summary>
    /// <param name="positionId">ID of the position to filter transactions by.</param>
    /// <returns>A <see cref="TransactionFilters" /> instance filtering by the specified position ID.</returns>
    public static TransactionFilters FromPositionId(int positionId)
    {
        return new TransactionFilters
        {
            PositionId = positionId
        };
    }

    /// <summary>
    ///     Creates an instance of <see cref="TransactionFilters" /> filtering by the specified instrument ID.
    /// </summary>
    /// <param name="instrumentId">ID of the instrument to filter transactions by.</param>
    /// <returns>A <see cref="TransactionFilters" /> instance filtering by the specified instrument ID.</returns>
    public static TransactionFilters FromInstrumentId(int instrumentId)
    {
        return new TransactionFilters
        {
            InstrumentId = instrumentId
        };
    }
}