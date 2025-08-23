using ClearBudget.Application.Client.Queries;
using ClearBudget.Database.Entities.Client;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;
using ICookieManager = ClearBudget.Infrastructure.Services.Cookie.ICookieManager;


namespace ClearBudget.Application.Services;

public interface ICurrentUserService
{
    Task<ClientUser?> GetAsync();
    Task<ClaimsPrincipal?> GetPrincipalAsync();
    Task<bool> IsAuthenticatedAsync();
    string? GetIpAddress();
    Task<bool> SignInAsync(ClientUser clientUser);
    Task SignOutAsync();
    Task<bool> HasClaimAsync(string type, string value);
    Task<bool> IsInRoleAsync(string name);
}

public class CurrentUserService(IMediator mediator,
    ICookieManager cookieManager,
    IHttpContextAccessor httpContextAccessor,
    HttpClient httpClient,
    AuthenticationStateProvider authenticationStateProvider) : ICurrentUserService
{

    public async Task<ClientUser?> GetAsync()
    {
        var principal = await authenticationStateProvider.GetAuthenticationStateAsync();

        var user = ToClientUser(principal.User);
        if (user == null) return null;

        var authenticationToken = cookieManager.Get("AuthenticationToken");
        if (!string.IsNullOrWhiteSpace(authenticationToken) && user.AuthenticationToken != authenticationToken) return null;
        return user;
    }

    public async Task<ClaimsPrincipal?> GetPrincipalAsync()
    {
        var principal = await authenticationStateProvider.GetAuthenticationStateAsync();
        if (!(principal.User?.Identity?.IsAuthenticated ?? false)) return null;
        return principal.User;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var principal = await authenticationStateProvider.GetAuthenticationStateAsync();
        return !(principal.User?.Identity?.IsAuthenticated ?? false);
    }

    public string? GetIpAddress()
    {
        return httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public async Task<bool> SignInAsync(ClientUser clientUser)
    {
        if (httpContextAccessor?.HttpContext is null) return false;
        if (clientUser == null || string.IsNullOrWhiteSpace(clientUser.AuthenticationToken)) return false;

        var claimsResult = await mediator.Send(new GetClientUserClaimsQuery { ClientUserId = clientUser.Id });
        var rolesResult = await mediator.Send(new GetClientUserRolesQuery { ClientUserId = clientUser.Id });

        var userIdentity = new ClaimsIdentity("CurrentUser_Core", ClaimTypes.Name, ClaimTypes.Role);
        userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, clientUser.Id.ToString()));
        userIdentity.AddClaim(new Claim(ClaimTypes.Name, clientUser.EmailAddress));
        userIdentity.AddClaim(new Claim(ClaimTypes.Email, clientUser.EmailAddress));
        userIdentity.AddClaim(new Claim("AuthenticationToken", clientUser.AuthenticationToken));
        userIdentity.AddClaim(new Claim("FirstName", clientUser.Forename));
        userIdentity.AddClaim(new Claim("LastName", clientUser.Surname));

        var authIdentity = new ClaimsIdentity("CurrentUser_Authentication", ClaimTypes.Name, ClaimTypes.Role);
        foreach (var claim in claimsResult.Claims.DistinctBy(x => x.Type))
        {
            authIdentity.AddClaim(new Claim(claim.Type, claim.Value));
        }

        foreach (var role in rolesResult.Roles.DistinctBy(x => x.Title))
        {
            authIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Title));
        }

        var userPrincipal = new ClaimsPrincipal([userIdentity, authIdentity]);

        if (httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            await SignOutAsync();

        await httpContextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            userPrincipal,
            new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60 * 8),
                IsPersistent = true,
                AllowRefresh = true,
                IssuedUtc = DateTime.UtcNow
            });

        cookieManager.Add("CurrentUser_AuthenticationToken", clientUser.AuthenticationToken, TimeSpan.FromDays(30), SameSiteMode.Strict, true);
        return true;
    }

    public async Task SignOutAsync()
    {
        cookieManager.Delete("CurrentUser_AuthenticationToken");
        await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<bool> HasClaimAsync(string type, string value)
    {
        var principal = await authenticationStateProvider.GetAuthenticationStateAsync();
        if (principal.User.Identity?.IsAuthenticated ?? false) return false;
        return principal.User.HasClaim(type, value);
    }
    public async Task<bool> IsInRoleAsync(string name)
    {
        var principal = await authenticationStateProvider.GetAuthenticationStateAsync();
        if (principal.User.Identity?.IsAuthenticated ?? false) return false;
        return principal.User.IsInRole(name);
    }

    private static ClientUser? ToClientUser(ClaimsPrincipal? p)
    {
        if (p?.Identity?.IsAuthenticated != true)
            return null;

        // You stored Id as ClaimTypes.NameIdentifier
        var id = p.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(id, out var gid))
            return null;

        return new ClientUser
        {
            Id = gid,
            EmailAddress = p.FindFirst(ClaimTypes.Email)?.Value ?? p.FindFirst(ClaimTypes.Name)?.Value ?? "",
            Forename = p.FindFirst("FirstName")?.Value ?? "",
            Surname = p.FindFirst("LastName")?.Value ?? "",
            AuthenticationToken = p.FindFirst("AuthenticationToken")?.Value
        };
    }

}