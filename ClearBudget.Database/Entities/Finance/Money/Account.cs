using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBudget.Database.Entities.Finance.Money
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string AccountName { get; set; }           // (e.g., "Lisa Money Box")
        public decimal GrossMoney { get; set; }           // Initial amount
        public decimal InterestRate { get; set; }         // Interest rate 
        public decimal Earnings { get; set; }             // Interest earned (over year with above)
        public decimal Net { get; set; }                  // Total amount after interest

        //something like this
        public Account (string accountName, decimal grossMoney, decimal interestRate)
        {
            AccountName = accountName;
            GrossMoney = grossMoney;
            InterestRate = interestRate;

            var earnings = grossMoney * InterestRate;
            Earnings = earnings;

            Net = GrossMoney + earnings;
        }  
    }
}
