namespace ClearBudget.Database.Entities.Transactions;

public class AccountTransaction : BaseEntity<Guid>
{
    public enum TransactionTypeEnum
    {
        Unknown,
        Income,
        Outcome,
        Interest,
        Tax
    }

    public enum TransactionRecurringTypeEnum
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Custom
    }

    public Guid AccountId { get; set; }
    public virtual Account Account { get; set; }

    public string Title { get; set; }
    public DateTime TransactionDate { get; set; }
    public TransactionTypeEnum Type { get; set; }
    public decimal Amount { get; set; }
    public TransactionRecurringTypeEnum Recurring { get; set; }
    public int? RecurringCustomDays { get; set; }

    public int Rank { get; set; }
    public bool? Essential { get; set; }

    public Guid? CategoryId { get; set; }
    public virtual AccountTransactionCategory? Category { get; set; }
}
