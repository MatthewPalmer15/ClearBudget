namespace ClearBudget.Database.Entities.Client;

public class ClientRoleClaim : BaseEntity<Guid>
{
    public Guid ClientRoleId { get; set; }
    public virtual ClientRole ClientRole { get; set; }

    public string Type { get; set; }
    public string Value { get; set; }
}