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
        public AccountTypeEnum Type { get; set; }
    }

    public class AccountOverview
    {
        public decimal SavingsTotal {  get; set; }
        public decimal IsasTotal {  get; set; }
        public decimal InvestmentsTotal {  get; set; }
        public decimal OtherTotal {  get; set; }
        public decimal Total {  get; set; }
        public decimal FutureTotal { get; set; }
    }

    
}

