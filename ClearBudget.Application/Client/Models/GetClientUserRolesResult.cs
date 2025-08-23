namespace ClearBudget.Application.Client.Models;

public class GetClientUserRolesResult
{
    public List<Role> Roles { get; set; }

    public class Role
    {
        public string Title { get; set; }
    }
}