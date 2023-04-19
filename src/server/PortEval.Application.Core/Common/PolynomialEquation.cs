using System;
using System.Collections.Generic;
using System.Linq;

namespace PortEval.Application.Core.Common;

/// <summary>
///     Represents a <see href="https://en.wikipedia.org/wiki/Newton%27s_method">Newton's method</see> polynomial equation.
/// </summary>
public class PolynomialEquation
{
    private const int IterationLimit = 5000;
    private readonly List<(int, double)> _coefficients;
    private readonly double _precision;

    /// <summary>
    ///     Initializes the equation with the specified approximation precision.
    /// </summary>
    /// <param name="precision">Approximation precision.</param>
    public PolynomialEquation(double precision)
    {
        _coefficients = new List<(int, double)>();
        _precision = precision;
    }

    /// <summary>
    ///     Adds a new coefficient to the equation.
    /// </summary>
    /// <param name="exponent">Coefficient exponent.</param>
    /// <param name="coefficient">Coefficient to add.</param>
    public void AddCoefficient(int exponent, double coefficient)
    {
        _coefficients.Add((exponent, coefficient));
    }

    /// <summary>
    ///     Calculates the approximate equation root using
    ///     <see href="https://en.wikipedia.org/wiki/Newton%27s_method">Newton's method</see>.
    /// </summary>
    /// <param name="initialGuess">Initial guess of the root.</param>
    /// <returns>A root approximation.</returns>
    public double CalculateRoot(double initialGuess)
    {
        var x = initialGuess;
        var previousX = x;
        var i = 0;
        while (i < IterationLimit)
        {
            x = previousX - ConvertToFunction(previousX) / CalculateDerivative(previousX);
            if (Math.Abs(x - previousX) <= _precision) return x;
            previousX = x;
            i++;
        }

        return x;
    }

    /// <summary>
    ///     Evaluates the <see href="https://en.wikipedia.org/wiki/Newton%27s_method">Newton's method's</see> f function based
    ///     on the supplied root approximation.
    /// </summary>
    /// <param name="x">Root approximation.</param>
    /// <returns>The function value.</returns>
    private double ConvertToFunction(double x)
    {
        return _coefficients.Select(t => Math.Pow(x, t.Item1) * t.Item2).Sum();
    }

    /// <summary>
    ///     Calculates the derivative of the <see href="https://en.wikipedia.org/wiki/Newton%27s_method">Newton's method</see>
    ///     's f function based on the supplied root approximation.
    /// </summary>
    /// <param name="x">Root approximation.</param>
    /// <returns>The derivative's value.</returns>
    private double CalculateDerivative(double x)
    {
        return _coefficients.Select(t => ApplyPowerRule(t.Item1, t.Item2, x)).Sum();
    }

    private double ApplyPowerRule(double exponent, double coefficient, double x)
    {
        if (exponent == 0) return 0;

        return exponent * coefficient * Math.Pow(x, exponent - 1);
    }
}