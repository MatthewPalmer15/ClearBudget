//Eg house, car, subscriptions ect
public class OutgoingCategory
{
    public Guid Id {get; set;}
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public decimal Total { get; set; }
    public List<ExpenseItem> Items { get; set; } = new();
}
