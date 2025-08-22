public class Salary
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public decimal Gross { get; set; }
    public decimal Deductables { get; set; }
    public decimal Net { get; set; }
    public List<Deduction> DeductionBreakdown { get; set; } = new();
}