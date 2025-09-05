using static ClearBudget.Database.Entities.Transactions.Account;

namespace ClearBudget.Application.Dashboard.Models;

public class GetClientUserDashboardResult
{
    public List<Account> accounts { get; set; } = [];
    public AccountOverview accountOverview { get; set; } = new AccountOverview();
    
    public class Account
    {
        public string Title { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal NetAmount { get; set; }
    }

    public class AccountOverview
    {
        public int Total {  get; set; }
        public int FutureTotal { get; set; }
    }
}

