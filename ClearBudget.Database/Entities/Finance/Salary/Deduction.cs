//Tax, NI, PENTION ECT Will need to create rules
public class Deduction
{
    public int DeductionId {get; set;}
    public int SalaryId {get; set;}
    public string Name { get; set; }
    public decimal Yearly { get; set; }
    public decimal Monthly { get; set; }
    public decimal Weekly { get; set; }
}
