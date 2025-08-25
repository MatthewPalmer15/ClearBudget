namespace ClearBudget.Database.Entities.Transactions;

public class AccountTransactionCategory : BaseEntity<Guid>
{
    public Guid? ParentId { get; set; }
    public virtual AccountTransactionCategory Parent { get; set; }

    public string Title { get; set; }
}
