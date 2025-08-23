using ClearBudget.Application.Dashboard.Models;
using System.Net.Http.Json;

namespace ClearBudget.Web.Client.Pages;

public partial class Dashboard : CustomClientComponent
{
    private List<GetClientUserDashboardResult.Transaction> Transactions { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {

        var response = await Http.GetFromJsonAsync<GetClientUserDashboardResult>($"api/dashboard/get", CancellationToken);
        if (response != null)
            Transactions = response.Transactions;
    }
}