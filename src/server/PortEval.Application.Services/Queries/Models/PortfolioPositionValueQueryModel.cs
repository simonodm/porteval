﻿namespace PortEval.Application.Services.Queries.Models
{
    internal class PortfolioPositionValueQueryModel
    {
        public string InstrumentCurrency { get; set; }
        public string PortfolioCurrency { get; set; }
        public decimal PositionValue { get; set; }
    }
}