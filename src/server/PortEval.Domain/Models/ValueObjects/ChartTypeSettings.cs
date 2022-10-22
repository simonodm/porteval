using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.ValueObjects
{
    public class ChartTypeSettings : ValueObject
    {
        public ChartType Type { get; private set; }
        public AggregationFrequency? Frequency { get; private set; }
        public string CurrencyCode { get; private set; }

        private ChartTypeSettings()
        {
        }

        public static ChartTypeSettings PriceChart(string currencyCode)
        {
            return new ChartTypeSettings
            {
                Type = ChartType.Price,
                CurrencyCode = currencyCode
            };
        }

        public static ChartTypeSettings ProfitChart(string currencyCode)
        {
            return new ChartTypeSettings
            {
                Type = ChartType.Profit,
                CurrencyCode = currencyCode
            };
        }

        public static ChartTypeSettings PerformanceChart()
        {
            return new ChartTypeSettings
            {
                Type = ChartType.Performance
            };
        }

        public static ChartTypeSettings AggregatedProfitChart(string currencyCode, AggregationFrequency frequency)
        {
            return new ChartTypeSettings
            {
                Type = ChartType.AggregatedProfit,
                CurrencyCode = currencyCode,
                Frequency = frequency
            };
        }

        public static ChartTypeSettings AggregatedPerformanceChart(AggregationFrequency frequency)
        {
            return new ChartTypeSettings
            {
                Type = ChartType.AggregatedPerformance,
                Frequency = frequency
            };
        }
    }
}
