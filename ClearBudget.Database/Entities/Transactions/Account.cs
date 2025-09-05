using ClearBudget.Database.Entities.Client;

namespace ClearBudget.Database.Entities.Transactions;

public class Account : BaseEntity<Guid>
{
    //public Guid ClientUserId { get; set; }
    //public virtual ClientUser ClientUser { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateClosed { get; set; }
    public string Title { get; set; }
    public AccountTypeEnum Type { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal InterestRate { get; set; }
    public decimal NetAmount => GrossAmount * (1 + (InterestRate / 100));
}