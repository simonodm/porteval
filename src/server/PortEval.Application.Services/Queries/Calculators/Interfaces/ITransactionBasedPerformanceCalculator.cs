using PortEval.Application.Services.Queries.Models;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Services.Queries.Calculators.Interfaces
{
    public interface ITransactionBasedPerformanceCalculator
    {
        public decimal CalculatePerformance(IEnumerable<TransactionDetailsQueryModel> transactions, DateTime from, DateTime to);
    }
}
