using ClearBudget.Application.Dashboard.Commands;
using ClearBudget.Application.Dashboard.Queries;
using ClearBudget.Web.Models.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace ClearBudget.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : CustomController
{
    [HttpGet("get")]
    public async Task<JsonResult> Get([FromQuery] GetDashboardRequest request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(new GetClientUserDashboardQuery { }, cancellationToken);
        return Json(response);
    }

    [HttpPost("post")]
    public async Task<IActionResult> Post([FromBody] CreateAccountCommand command, CancellationToken cancellationToken = default)
    {
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

}