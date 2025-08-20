namespace ClearBudget.Application.Client.Models;

public class GetCurrentUserResult
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
}