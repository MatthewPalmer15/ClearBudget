namespace ClearBudget.Database.Entities.Investments
{
    public class Investment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public decimal InvestmentAmount { get; set; }
        public decimal Units { get; set; }
        public decimal CostPerUnit { get; set; }    // can calculate this at run time
        public decimal CurrentPricePerUnit { get; set; } // could maybe find this information somewhere idk?
        public decimal SellAmount { get; set; }     // calculate run time
        public decimal Profit { get; set; }
        public decimal PercentChange { get; set; }
    }
}
