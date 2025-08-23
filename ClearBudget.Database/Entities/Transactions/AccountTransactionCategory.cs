namespace ClearBudget.Database.Entities.Transactions;

public class AccountTransactionCategory : BaseEntity<Guid>
{
    public string Title { get; set; }
}
