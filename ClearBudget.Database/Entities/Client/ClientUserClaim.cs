namespace ClearBudget.Database.Entities.Client;

public class ClientUserClaim : BaseEntity<Guid>
{
    public Guid ClientUserId { get; set; }
    public virtual ClientUser ClientUser { get; set; }

    public string Type { get; set; }
    public string Value { get; set; }
}