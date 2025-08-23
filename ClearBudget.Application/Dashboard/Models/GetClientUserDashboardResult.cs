namespace ClearBudget.Application.Dashboard.Models;

public class GetClientUserDashboardResult
{
    public List<Transaction> Transactions { get; set; } = [];
    public class Transaction
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}

