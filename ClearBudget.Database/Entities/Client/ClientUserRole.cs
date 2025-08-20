namespace ClearBudget.Database.Entities.Client;

public class ClientUserRole : BaseEntity<Guid>
{
    public Guid ClientUserId { get; set; }
    public virtual ClientUser ClientUser { get; set; }

    public Guid ClientRoleId { get; set; }
    public virtual ClientRole ClientRole { get; set; }
}