public class ExpenseItem
{
    public int ExpenseItemId { get; set; }
    public int OutgoingCategoryId {get;set;}
    public string Name { get; set; }
    public decimal Yearly { get; set; }
    public decimal Monthly { get; set; }
    public decimal Weekly { get; set; }
}