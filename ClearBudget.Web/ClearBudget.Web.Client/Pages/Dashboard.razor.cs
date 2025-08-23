using ClearBudget.Application.Dashboard.Models;
using ClearBudget.Application.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace ClearBudget.Web.Client.Pages;

public partial class Dashboard : CustomClientComponent
{
    private List<GetClientUserDashboardResult.Transaction> Transactions { get; set; } = [];

    [Inject]
    public ICurrentUserService CurrentUserService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!await CurrentUserService.IsAuthenticatedAsync())
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        var currentUser = await CurrentUserService.GetAsync();

        var currentUserId = Guid.NewGuid();

        var response = await Http.GetFromJsonAsync<GetClientUserDashboardResult>($"api/dashboard/get?clientUserId={currentUserId}", CancellationToken);
        if (response != null)
            Transactions = response.Transactions;


    }
}