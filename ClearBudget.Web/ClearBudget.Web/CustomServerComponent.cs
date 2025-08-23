using MediatR;
using Microsoft.AspNetCore.Components;

namespace ClearBudget.Web;

public class CustomServerComponent : ComponentBase
{
    private readonly CancellationTokenSource _cts = new();
    protected CancellationToken CancellationToken => _cts.Token;
    [Inject] public IMediator Mediator { get; set; } = null!;
}