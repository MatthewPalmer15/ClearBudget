using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBudget.Database.Entities.Finance.Money
{
    public class FinanceSummary
    {
        public List<Account> Accounts { get; set; } = new List<Account>();
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountAddingIntrest { get; set; }
        public decimal TotalNetPlusWork { get; set; }

        public FinanceSummary(decimal totalAmount, decimal totalNet, decimal totalNetPlusWork)
        {
            TotalAmount = totalAmount;
            TotalAmountAddingIntrest = totalNet;
            TotalNetPlusWork = totalNetPlusWork;
        }
    }
}
