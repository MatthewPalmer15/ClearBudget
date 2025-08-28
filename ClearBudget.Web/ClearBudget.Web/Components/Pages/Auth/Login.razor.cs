using ClearBudget.Application.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace ClearBudget.Web.Components.Pages.Auth;

public partial class Login : CustomServerComponent
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public ICurrentUserService CurrentUserService { get; set; }

    private LoginEditModel _model = new();

    private async Task OnLoginFormSubmit()
    {
        var hasSuccessfullyLoggedIn = await CurrentUserService.SignInAsync(_model.EmailAddress, _model.Password, CancellationToken);
        if (hasSuccessfullyLoggedIn)
        {
            NavigationManager.NavigateTo("/", forceLoad: true);
            Snackbar.Add("Login successful!", Severity.Success);
            return;
        }

        Snackbar.Add("Failed to log in", Severity.Error);
    }

    public class LoginEditModel
    {
        [Required, EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}