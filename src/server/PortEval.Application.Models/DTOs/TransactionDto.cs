using System;

namespace PortEval.Application.Models.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public int PortfolioId { get; set; }
        public DateTime Time { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public string Note { get; set; }
    }
}
