namespace ClearBudget.Database.Entities.Client;

public class ClientUserLogin : BaseEntity<Guid>
{
    public Guid ClientRoleId { get; set; }
    public virtual ClientRole ClientRole { get; set; }

    public string LoginProvider { get; set; }
    public string ProviderKey { get; set; }
    public string ProviderDisplayName { get; set; }
}