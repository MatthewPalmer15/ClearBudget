using System.Collections.Generic;

namespace PersonalFinance.Models
{
    public enum UKStudentLoanType
    {
        Plan1Undergraduate,
        Plan2Undergraduate,
        Plan4Undergraduate,
        LegacyUndergraduate,
        MastersPostgraduate,
        DoctoralPostgraduate
    }

    public class Salary
    {
        //Account Info
        //public Guid AccountId { get; set; }
        //public virtual Account Account { get; set; }

        // Core Salary Information
        public decimal GrossIncome { get; set; }

        // Student Loan Information
        public bool HasStudentLoan { get; set; }
        public UKStudentLoanType? StudentLoanType { get; set; }

        // Pension Contributions
        public decimal? PensionContributionPercentage { get; set; }

        // Overtime and Bonus
        public decimal? OvertimeHours { get; set; }
        public decimal? OvertimeRate { get; set; }
        public decimal? Bonus { get; set; }

        public bool PaysNationalInsurance { get; set; }
    }
}
