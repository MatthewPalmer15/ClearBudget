namespace ClearBudget.Database.Entities.Client;

public class ClientRole : BaseEntity<Guid>
{
    public string Name { get; set; }
}