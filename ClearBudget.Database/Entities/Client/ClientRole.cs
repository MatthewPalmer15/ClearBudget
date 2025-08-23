namespace ClearBudget.Database.Entities.Client;

public class ClientRole : BaseEntity<Guid>
{
    public string Name { get; set; }
    public virtual List<ClientRoleClaim> Claims { get; set; }
}