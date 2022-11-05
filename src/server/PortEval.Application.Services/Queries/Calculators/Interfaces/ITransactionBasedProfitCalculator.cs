using PortEval.Application.Services.Queries.Models;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Services.Queries.Calculators.Interfaces
{
    internal interface ITransactionBasedProfitCalculator
    {
        public decimal CalculateProfit(IEnumerable<TransactionDetailsQueryModel> transactions, DateTime from, DateTime to);
    }
}
