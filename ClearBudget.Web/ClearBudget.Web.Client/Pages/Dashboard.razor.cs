using ClearBudget.Application.Dashboard.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace ClearBudget.Web.Client.Pages;

public partial class Dashboard : ComponentBase
{
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;

    [Inject]
    public HttpClient HttpClient { get; set; }

    private List<GetClientUserDashboardResult.Transaction> Transactions { get; set; } = [];

    protected async override Task OnInitializedAsync()
    {
        var currentUserId = Guid.NewGuid();

        var response = await HttpClient.GetFromJsonAsync<GetClientUserDashboardResult>($"api/dashboard/get?clientUserId={currentUserId}", _cancellationToken);
        if (response != null)
            Transactions = response.Transactions;
    }
}