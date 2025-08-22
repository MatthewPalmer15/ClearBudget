//Eg house, car, subscriptions ect
public class OutgoingCategory
{
    public int OutgoingCategoryId {get; set;}
    public int UserId {get;set}
    public string Name { get; set; }
    public decimal Total { get; set; }
    public List<ExpenseItem> Items { get; set; } = new();
}
