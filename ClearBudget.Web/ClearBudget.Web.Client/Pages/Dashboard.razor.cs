using ClearBudget.Application.Dashboard.Models;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace ClearBudget.Web.Client.Pages;

public partial class Dashboard : CustomClientComponent
{
    private GetClientUserDashboardResult Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var response = await Http.GetFromJsonAsync<GetClientUserDashboardResult>("api/dashboard/get",CancellationToken);

        if (response != null)
            Model = response;
    }
}
