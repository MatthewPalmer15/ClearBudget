namespace ClearBudget.Application.Finance.Models;

public class GetClientFinanceResult
{
   public Salary salary {get; set;}
   public OutgoingCategory OutGoings {get; set;}
}

public class Salary
{
    public decimal Gross { get; set; }
    public decimal NationalInsuranceContributions { get; private set; }
    public decimal TaxContributions { get; private set; }
    public decimal StudentLoanContributions { get; private set; }
    public decimal PensionContributionPercentage { get; set; } // e.g., 5% as 5

    public decimal PensionContribution => Gross * (PensionContributionPercentage / 100);

    public decimal NetSalary => Gross - TaxContributions - NationalInsuranceContributions - StudentLoanContributions - PensionContribution;

    // Method to calculate contributions
    public void CalculateContributions()
    {
        TaxContributions = CalculateIncomeTax(Gross);
        NationalInsuranceContributions = CalculateNI(Gross);
        StudentLoanContributions = CalculateStudentLoan(Gross);
    }

    //need to revisit as this shouldnt be done on gross 
    private decimal CalculateIncomeTax(decimal gross)
    {
        decimal tax = 0;
        decimal personalAllowance = 12570m;

        if (gross <= personalAllowance)
            return 0;

        decimal taxable = gross - personalAllowance;

        if (taxable <= 37700m)
            tax = taxable * 0.20m; 
        else if (taxable <= 125140m)
            tax = 37700m * 0.20m + (taxable - 37700m) * 0.40m; 
        else
            tax = 37700m * 0.20m + (125140m - 37700m) * 0.40m + (taxable - 125140m) * 0.45m; 

        return tax;
    }

    //need to revisit as some people dont pay NI
    private decimal CalculateNI(decimal gross)
    {
        decimal ni = 0;
        decimal lowerLimit = 12570m;
        decimal upperLimit = 50270m;

        if (gross <= lowerLimit)
            return 0;

        decimal niPayable = Math.Min(gross, upperLimit) - lowerLimit;
        ni = niPayable * 0.12m;

        if (gross > upperLimit)
            ni += (gross - upperLimit) * 0.02m;

        return ni;
    }

    //need to revist to calculate based on diffrent student loans
    private decimal CalculateStudentLoan(decimal gross)
    {
        decimal threshold = 27295m;
        if (gross <= threshold)
            return 0;

        return (gross - threshold) * 0.09m; 
    }
}

