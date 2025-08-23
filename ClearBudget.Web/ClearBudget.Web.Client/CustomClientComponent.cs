using Microsoft.AspNetCore.Components;

namespace ClearBudget.Web.Client;

public class CustomClientComponent : ComponentBase
{
    private readonly CancellationTokenSource _cts = new();
    protected CancellationToken CancellationToken => _cts.Token;

    [Inject] public HttpClient Http { get; set; } = null!;

}