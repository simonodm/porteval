using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Services;

/// <summary>
///     A domain service implementing operations on application currencies.
/// </summary>
public interface ICurrencyDomainService
{
    /// <summary>
    ///     Changes the default currency.
    /// </summary>
    /// <param name="previousDefaultCurrency">Previous default currency.</param>
    /// <param name="newDefaultCurrency">New default currency.</param>
    public void ChangeDefaultCurrency(Currency previousDefaultCurrency, Currency newDefaultCurrency);
}