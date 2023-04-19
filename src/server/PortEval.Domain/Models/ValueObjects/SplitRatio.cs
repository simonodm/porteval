namespace PortEval.Domain.Models.ValueObjects;

/// <summary>
///     Represents the ratio of an instrument split.
/// </summary>
public class SplitRatio : ValueObject
{
    /// <summary>
    ///     The denominator of the factor by which the number of shares is multiplied after a stock split.
    ///     For example, if a 5-for-1 stock split happens, this value will be equal to 1.
    /// </summary>
    public int Denominator { get; }

    /// <summary>
    ///     The numerator of the factor by which the number of shares is multiplied after a stock split.
    ///     For example, if a 5-for-1 stock split happens, this value will be equal to 5.
    /// </summary>
    public int Numerator { get; }

    /// <summary>
    ///     Represents the result factor by which the number of shares in circulation is multiplied after a split.
    /// </summary>
    public decimal Factor => (decimal)Numerator / Denominator;

    /// <summary>
    ///     Initializes the split ratio with the provided numerator and denominator.
    /// </summary>
    /// <param name="denominator">Denominator of the ratio.</param>
    /// <param name="numerator">Numerator of the ratio.</param>
    public SplitRatio(int denominator, int numerator)
    {
        Denominator = denominator;
        Numerator = numerator;
    }
}