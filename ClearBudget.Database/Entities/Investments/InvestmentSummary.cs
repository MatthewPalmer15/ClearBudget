namespace ClearBudget.Database.Entities.Investments
{
    public class InvestmentSummary
    {
        public List<Investment> Investments { get; set; } = new List<Investment>();
        public decimal TotalInvested { get; set; }
        public decimal TotalSellValue { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalIncreasePercent { get; set; }
    }
}
