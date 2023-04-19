namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents a stock exchange.
/// </summary>
public class Exchange : VersionedEntity, IAggregateRoot
{
    /// <summary>
    ///     The stock exchange's ticker symbol.
    /// </summary>
    public string Symbol { get; private set; }

    /// <summary>
    ///     The name of the stock exchange.
    /// </summary>
    public string Name { get; private set; }

    internal Exchange(string symbol)
    {
        Symbol = symbol;
    }

    internal Exchange(string symbol, string name)
    {
        Symbol = symbol;
        Name = name;
    }

    /// <summary>
    ///     A factory method for creating a stock exchange.
    /// </summary>
    /// <param name="symbol">The ticker symbol of the stock exchange.</param>
    /// <returns>The newly created stock exchange.</returns>
    public static Exchange Create(string symbol)
    {
        return new Exchange(symbol);
    }

    /// <summary>
    ///     A factory method for creating a stock exchange.
    /// </summary>
    /// <param name="symbol">The ticker symbol of the stock exchange.</param>
    /// <param name="name">The name of the stock exchange.</param>
    /// <returns>The newly created stock exchange.</returns>
    public static Exchange Create(string symbol, string name)
    {
        return new Exchange(symbol, name);
    }

    /// <summary>
    ///     Renames the stock exchange.
    /// </summary>
    /// <param name="newName">The new name of the stock exchange.</param>
    public void Rename(string newName)
    {
        Name = newName;
    }
}