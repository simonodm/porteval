namespace PortEval.Domain.Models.ValueObjects
{
    public class SplitRatio : ValueObject
    {
        /// <summary>
        /// The denominator of the factor by which the number of shares is multiplied after a stock split.
        /// For example, if a 5-for-1 stock split happens, this value will be equal to 1.
        /// </summary>
        public int Denominator { get; private set; }

        /// <summary>
        /// The numerator of the factor by which the number of shares is multiplied after a stock split.
        /// For example, if a 5-for-1 stock split happens, this value will be equal to 5.
        /// </summary>
        public int Numerator { get; private set; }

        /// <summary>
        /// Represents the result factor by which the number of shares in circulation is multiplied after a split.
        /// </summary>
        public decimal Factor => (decimal)Numerator / Denominator;

        public SplitRatio(int denominator, int numerator)
        {
            Denominator = denominator;
            Numerator = numerator;
        }
    }
}
