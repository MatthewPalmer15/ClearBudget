using System.ComponentModel.DataAnnotations;

namespace ClearBudget.Web.Components.Pages.Auth;

public partial class Login : CustomServerComponent
{
    private LoginModel loginModel = new();

    private void HandleValidSubmit()
    {
        Navigation.NavigateTo("/");
    }

    public class LoginModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}