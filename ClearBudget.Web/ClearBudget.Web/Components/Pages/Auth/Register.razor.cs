using ClearBudget.Application.Client.Commands;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace ClearBudget.Web.Components.Pages.Auth;

public partial class Register : CustomServerComponent
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    private RegisterEditModel _model = new();

    private async Task OnRegisterFormSubmit()
    {
        var response = await Mediator.Send(new RegisterUserCommand
        {
            FirstName = _model.FirstName,
            LastName = _model.LastName,
            EmailAddress = _model.EmailAddress,
            Password = _model.Password,
            ConfirmPassword = _model.ConfirmPassword
        }, CancellationToken);

        if (response.Success)
        {
            NavigationManager.NavigateTo("/");
            Snackbar.Add("Registration successful!", Severity.Success);
            return;
        }

        Snackbar.Add(string.Join(",", response.Errors.Select(x => x.ErrorMessage)), Severity.Error);
    }

    public class RegisterEditModel
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
    }
}