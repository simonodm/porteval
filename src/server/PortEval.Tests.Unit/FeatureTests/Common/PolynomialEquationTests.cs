using PortEval.Application.Features.Common;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Common
{
    public class PolynomialEquationTests
    {
        [Theory]
        [InlineData(new[] { 0d, 1d }, 0, 0.01, 0)] // x = 0
        [InlineData(new[] { 1d, 1d }, -1, 0.01, 0)] // x + 1 = 0
        [InlineData(new[] { 0d, -4d }, 0, 0.01, 0)] // -4x = 0
        [InlineData(new[] { 1d, -2d, 1d }, 1, 0.01, 0)] // x^2 - 2x + 1 = 0
        [InlineData(new[] { -4d, -2d, 1d }, 3.23, 0.01, 2)] // x^2 - 2x - 4 = 0
        [InlineData(new[] { 1d, -2d, 1d, 7d }, -0.76, 0.01, 0)] // 7x^3 + x^2 - 2x + 1 = 0
        [InlineData(new[] { 0d, -4d, 0d, -3d, 0d, 0d, 1d }, 1.65, 0.01, 1.5)] // x^6 - 3x^3 - 4x = 0
        [InlineData(new[] { 0d, -4d, 0d, -3d, 0d, 0d, 1d }, 0, 0.01, 1)] // x^6 - 3x^3 - 4x = 0
        public void CalculateRoot_ReturnsApproximatelyCorrectRoot(double[] coefficients, double expectedRoot, double precision, double initialGuess)
        {
            var equation = new PolynomialEquation(precision);
            for (int i = 0; i < coefficients.Length; i++)
            {
                equation.AddCoefficient(i, coefficients[i]);
            }

            var result = equation.CalculateRoot(initialGuess);

            Assert.InRange(result, expectedRoot - precision, expectedRoot + precision);
        }
    }
}
