using ClearBudget.Application.Services;
using Microsoft.AspNetCore.Components;

namespace ClearBudget.Web.Components.Pages;

public partial class Home : CustomServerComponent
{
    [Inject]
    public ICurrentUserService CurrentUserService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var currentUser = await CurrentUserService.GetAsync();
    }

}