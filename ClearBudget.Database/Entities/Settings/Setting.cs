namespace ClearBudget.Database.Entities.Settings;

public class Setting : BaseEntity<Guid>
{
    public string Key { get; set; }
    public string Value { get; set; }
}
