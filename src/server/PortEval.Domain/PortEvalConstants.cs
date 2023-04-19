using System;

namespace PortEval.Domain;

/// <summary>
///     A static collection of domain constants.
/// </summary>
public static class PortEvalConstants
{
    /// <summary>
    ///     The earliest supported time for financial data.
    /// </summary>
    public static readonly DateTime FinancialDataStartTime = new(2000, 1, 1, 0, 0, 0);
}