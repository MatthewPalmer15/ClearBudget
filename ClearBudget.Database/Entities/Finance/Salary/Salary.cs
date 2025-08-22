public class Salary
{
    public int SalaryId {get; set;}
    public int UserId {get; set;}
    public decimal Gross { get; set; }
    public decimal Deductables { get; set; }
    public decimal Net { get; set; }
    public List<Deduction> DeductionBreakdown { get; set; } = new();
}