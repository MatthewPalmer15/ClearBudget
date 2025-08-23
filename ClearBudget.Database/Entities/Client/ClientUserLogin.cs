namespace ClearBudget.Database.Entities.Client;

public class ClientUserLogin : BaseEntity<Guid>
{
    public Guid ClientUserId { get; set; }
    public virtual ClientUser ClientUser { get; set; }

    public string Provider { get; set; }
    public string ProviderKey { get; set; }
    public string? ProviderDisplayName { get; set; }
}