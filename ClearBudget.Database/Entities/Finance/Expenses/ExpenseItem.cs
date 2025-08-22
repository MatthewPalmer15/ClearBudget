public class ExpenseItem
{
    public Guid Id { get; set; }
    public Guid OutgoingCategoryId {get;set;}
    public string Name { get; set; }
    public decimal Yearly { get; set; }
    public decimal Monthly { get; set; }
    public decimal Weekly { get; set; }
}