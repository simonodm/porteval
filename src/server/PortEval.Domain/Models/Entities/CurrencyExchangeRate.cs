using System;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents an exchange rate between two currencies.
/// </summary>
public class CurrencyExchangeRate : Entity, IAggregateRoot
{
    /// <summary>
    ///     ID of the exchange rate.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Time of the exchange rate.
    /// </summary>
    public DateTime Time { get; private set; }

    /// <summary>
    ///     The exchange rate.
    /// </summary>
    public decimal ExchangeRate { get; private set; }

    /// <summary>
    ///     Base currency.
    /// </summary>
    public string CurrencyFromCode { get; private set; }

    /// <summary>
    ///     Target currency.
    /// </summary>
    public string CurrencyToCode { get; private set; }

    internal CurrencyExchangeRate(int id, DateTime time, decimal exchangeRate, string currencyFromCode,
        string currencyToCode) : this(time, exchangeRate, currencyFromCode, currencyToCode)
    {
        Id = id;
    }

    internal CurrencyExchangeRate(DateTime time, decimal exchangeRate, string currencyFromCode, string currencyToCode)
    {
        if (time < PortEvalConstants.FinancialDataStartTime)
            throw new InvalidOperationException(
                $"Exchange rate time must be later than {PortEvalConstants.FinancialDataStartTime}");

        Time = time;
        ExchangeRate = exchangeRate;
        CurrencyFromCode = currencyFromCode;
        CurrencyToCode = currencyToCode;
    }

    /// <summary>
    ///     A factory method for creating a currency exchange rate.
    /// </summary>
    /// <param name="time">Time of the exchange rate.</param>
    /// <param name="exchangeRate">The exchange rate.</param>
    /// <param name="currencyFrom">Base currency.</param>
    /// <param name="currencyTo">Target currency.</param>
    /// <returns>The newly created exchange rate entity.</returns>
    public static CurrencyExchangeRate Create(DateTime time, decimal exchangeRate, Currency currencyFrom,
        Currency currencyTo)
    {
        return new CurrencyExchangeRate(time, exchangeRate, currencyFrom.Code, currencyTo.Code);
    }
}