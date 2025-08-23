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
        var response = await Mediator.Send(new GetClientUserDashboardQuery { ClientUserId = request.ClientUserId }, cancellationToken);
        return Json(response);
    }
}