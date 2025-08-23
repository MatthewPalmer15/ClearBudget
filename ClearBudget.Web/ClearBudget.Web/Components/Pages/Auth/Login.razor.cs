using ClearBudget.Application.Client.Commands;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace ClearBudget.Web.Components.Pages.Auth;

public partial class Login : CustomServerComponent
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    private LoginEditModel _model = new();

    private async Task OnLoginFormSubmit()
    {
        var response = await Mediator.Send(new LoginUserCommand
        {
            EmailAddress = _model.EmailAddress,
            Password = _model.Password,
        }, CancellationToken);

        if (response.Success)
        {
            NavigationManager.NavigateTo("/");
            Snackbar.Add("Login successful!", Severity.Success);
            return;
        }

        Snackbar.Add(string.Join(",", response.Errors.Select(x => x.ErrorMessage)), Severity.Error);
    }

    public class LoginEditModel
    {
        [Required, EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}