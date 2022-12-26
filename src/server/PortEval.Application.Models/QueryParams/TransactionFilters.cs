namespace PortEval.Application.Models.QueryParams
{
    public class TransactionFilters
    {
        public int? PortfolioId { get; set; }
        public int? PositionId { get; set; }
        public int? InstrumentId { get; set; }

        public static TransactionFilters FromPortfolioId(int portfolioId)
        {
            return new TransactionFilters
            {
                PortfolioId = portfolioId
            };
        }

        public static TransactionFilters FromPositionId(int positionId)
        {
            return new TransactionFilters
            {
                PositionId = positionId
            };
        }

        public static TransactionFilters FromInstrumentId(int instrumentId)
        {
            return new TransactionFilters
            {
                InstrumentId = instrumentId
            };
        }
    }
}
