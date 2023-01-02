﻿namespace PortEval.Application.Features.Queries.Models
{
    internal class PortfolioTransactionDetailsQueryModel : TransactionDetailsQueryModel
    {
        public int InstrumentId { get; set; }
        public string TransactionCurrency { get; set; }
        public string PortfolioCurrency { get; set; }
    }
}