using MediatR;
using Microsoft.AspNetCore.Components;

namespace ClearBudget.Web;

public class CustomComponentBase : ComponentBase
{
    [Inject] public IMediator Mediator { get; set; } = null!;
}