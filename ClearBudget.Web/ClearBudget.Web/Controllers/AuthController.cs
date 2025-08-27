using ClearBudget.Application.Client.Commands;
using ClearBudget.Application.Services;
using ClearBudget.Infrastructure.Encryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearBudget.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ICurrentUserService currentUserService, IEncryptionService encryptionService) : CustomController
{
    [Authorize]
    [HttpGet("get")]
    public async Task<JsonResult> Get(CancellationToken cancellationToken = default)
    {
        var user = await currentUserService.GetAsync(cancellationToken);
        return Json(user);
    }

    [HttpPost("login")]
    public async Task<JsonResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(new LoginUserCommand
        {
            EmailAddress = encryptionService.Decrypt(request.EmailAddress),
            Password = encryptionService.Decrypt(request.Password)
        }, cancellationToken);
        return Json(response);
    }

    [HttpPost("register")]
    public async Task<JsonResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(new RegisterUserCommand
        {
            FirstName = encryptionService.Decrypt(request.Forename),
            LastName = encryptionService.Decrypt(request.Surname),
            EmailAddress = encryptionService.Decrypt(request.EmailAddress),
            Password = encryptionService.Decrypt(request.Password),
            ConfirmPassword = encryptionService.Decrypt(request.ConfirmPassword)
        }, cancellationToken);
        return Json(response);
    }

    [Authorize]
    [HttpPost("hasclaim")]
    public async Task<JsonResult> HasClaim([FromBody] HasClaimRequest request, CancellationToken cancellationToken = default)
    {
        var response = await currentUserService.HasClaimAsync(request.Type, request.Value, cancellationToken);
        return Json(response);
    }

    [Authorize]
    [HttpPost("isinrole")]
    public async Task<JsonResult> IsInRole([FromBody] IsInRoleRequest request, CancellationToken cancellationToken = default)
    {
        var response = await currentUserService.IsInRoleAsync(request.RoleName, cancellationToken);
        return Json(response);
    }
}

public class LoginRequest
{
    public string EmailAddress { get; set; }
    public string Password { get; set; }
}

public class RegisterRequest
{
    public string Forename { get; set; }
    public string Surname { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class HasClaimRequest
{
    public string Type { get; set; }
    public string Value { get; set; }
}

public class IsInRoleRequest
{
    public string RoleName { get; set; }
}