namespace ClearBudget.Database.Entities.Client;

public class ClientUser : BaseEntity<Guid>
{
    public string Forename { get; set; }
    public string Surname { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorKey { get; set; }
    public string? AuthenticationToken { get; set; }
    public DateTime? AuthenticationTokenGenerated { get; set; }
    public DateTime? AuthenticationTokenExpiry { get; set; }
    public int MaxLoginAttempts { get; set; }
    public int FailedLoginAttempts { get; set; }
}