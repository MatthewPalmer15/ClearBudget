using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClearBudget.Web;

public class CustomController : Controller
{
    private IMediator? _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
}