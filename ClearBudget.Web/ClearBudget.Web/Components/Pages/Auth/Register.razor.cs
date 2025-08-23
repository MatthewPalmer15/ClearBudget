using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace ClearBudget.Web.Components.Pages.Auth;

public partial class Register : CustomServerComponent
{
    private RegisterModel registerModel = new();

    private void HandleValidSubmit()
    {
        Snackbar.Add("Registration successful!", Severity.Success);
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required, EmailAddress(ErrorMessage = "Invalid email")]
        public string EmailAddress { get; set; }

        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        public string ConfirmPassword { get; set; }

        public bool SignInUserAfterCreation { get; set; }
    }
}