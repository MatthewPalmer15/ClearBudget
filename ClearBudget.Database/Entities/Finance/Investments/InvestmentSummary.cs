using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBudget.Database.Entities.Finance.Investments
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
