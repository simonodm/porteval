namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents an investment portfolio.
/// </summary>
public class Portfolio : VersionedEntity, IAggregateRoot
{
    /// <summary>
    ///     ID of the portfolio.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     3-letter code of the currency in which the portfolio is being tracked.
    /// </summary>
    public string CurrencyCode { get; private set; }

    /// <summary>
    ///     Name of the portfolio.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Note of the portfolio.
    /// </summary>
    public string Note { get; private set; }

    internal Portfolio(int id, string name, string note, string currencyCode) : this(name, note, currencyCode)
    {
        Id = id;
    }

    internal Portfolio(string name, string note, string currencyCode)
    {
        Name = name;
        Note = note;
        CurrencyCode = currencyCode;
    }

    /// <summary>
    ///     A factory method for creating portfolios.
    /// </summary>
    /// <param name="name">Name of the portfolio.</param>
    /// <param name="note">Note of the portfolio.</param>
    /// <param name="currencyCode">3-letter code of the currency in which the portfolio is tracked.</param>
    /// <returns>The newly created portfolio entity.</returns>
    public static Portfolio Create(string name, string note, string currencyCode)
    {
        return new Portfolio(name, note, currencyCode);
    }

    /// <summary>
    ///     Renames the portfolio.
    /// </summary>
    /// <param name="name">The new name of the portfolio.</param>
    public void Rename(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Changes the note of the portfolio.
    /// </summary>
    /// <param name="note">The new note of the portfolio.</param>
    public void SetNote(string note)
    {
        Note = note;
    }

    /// <summary>
    ///     Changes the portfolio currency.
    /// </summary>
    /// <param name="currencyCode">3-letter code of the new portfolio currency.</param>
    public void ChangeCurrency(string currencyCode)
    {
        CurrencyCode = currencyCode;
    }
}