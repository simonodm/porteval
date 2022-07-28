namespace PortEval.Application.Models.DTOs
{
    public class EntityStatisticsDto
    {
        public decimal TotalPerformance { get; set; }
        public decimal LastMonthPerformance { get; set; }
        public decimal LastWeekPerformance { get; set; }
        public decimal LastDayPerformance { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal LastMonthProfit { get; set; }
        public decimal LastWeekProfit { get; set; }
        public decimal LastDayProfit { get; set; }
    }
}
