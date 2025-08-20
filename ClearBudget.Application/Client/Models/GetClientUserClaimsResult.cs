namespace ClearBudget.Application.Client.Models;

public class GetClientUserClaimsResult
{
    public List<Claim> Claims { get; set; }

    public class Claim
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public bool FromRole { get; set; }
    }
}