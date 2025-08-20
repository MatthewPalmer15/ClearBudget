using ClearBudget.Application.Client.Models;
using ClearBudget.Application.Client.Queries;
using ClearBudget.Database.Entities.Client;
using ClearBudget.Infrastructure.Services.Session;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;
using ICookieManager = ClearBudget.Infrastructure.Services.Cookie.ICookieManager;


namespace ClearBudget.Application.Services;

public interface ICurrentUserService
{
    ClientUser? Get();
    bool IsAuthenticated();
    string? GetIpAddress();
    Task SignInAsync(ClientUser clientUser, CancellationToken cancellationToken = default);
    Task SignOutAsync(bool clearSession = false, CancellationToken cancellationToken = default);
    Task<bool> HasClaimAsync(string type, string value, CancellationToken cancellationToken = default);
}

public class CurrentUserService(
    IMediator mediator,
    ISessionManager sessionManager,
    ICookieManager cookieManager,
    IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public ClientUser? Get()
    {

        var user = sessionManager.Get<ClientUser>("CurrentUser");
        if (user != null)
        {
            var authenticationToken = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "AuthenticationToken")?.Value;
            //var authenticationToken = cookieManager.Get("CurrentUser_AuthenticationToken");
            if (string.IsNullOrWhiteSpace(authenticationToken) || user.AuthenticationToken == authenticationToken)
                return user;
        }

        var claimsPrincipal = httpContextAccessor.HttpContext?.User;
        if (claimsPrincipal?.Identity?.IsAuthenticated ?? false)
        {
            user = new ClientUser
            {
                AuthenticationToken = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "AuthenticationToken")?.Value,
                Forename = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "FirstName")?.Value ?? string.Empty,
                Surname = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "LastName")?.Value ?? string.Empty,
                EmailAddress = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "EmailAddress")?.Value ?? string.Empty,
            };

            sessionManager.Add("CurrentUser", user);
            return user;
        }

        return null;
    }

    public bool IsAuthenticated()
    {
        return httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public string? GetIpAddress()
    {
        return httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public async Task SignInAsync(ClientUser clientUser, CancellationToken cancellationToken = default)
    {
        if (clientUser == null || string.IsNullOrWhiteSpace(clientUser.AuthenticationToken))
            return;

        // var claims = new List<Claim> {
        //     new(ClaimTypes.Name, clientUser.AuthenticationToken, ClaimValueTypes.String),
        //     new(ClaimTypes.Email, clientUser.EmailAddress, ClaimValueTypes.Email),
        //     new Claim("IsAppUser", ((httpContextAccessor.HttpContext?.Request?.Query["utm_medium"].ToString() ?? "") == "PWA").ToString()),
        // };

        cookieManager.Add("CurrentUser_AuthenticationToken", clientUser.AuthenticationToken, TimeSpan.FromDays(30),
            SameSiteMode.Strict, true);
        sessionManager.Add("CurrentUser", clientUser);

        var userIdentity = new ClaimsIdentity("CurrentUser_Login");
        userIdentity.AddClaim(new Claim("Id", clientUser.Id.ToString()));
        userIdentity.AddClaim(new Claim("EmailAddress", clientUser.EmailAddress));
        userIdentity.AddClaim(new Claim("IsAppUser",
            ((httpContextAccessor.HttpContext?.Request?.Query["utm_medium"].ToString() ?? "") == "PWA").ToString()));

        // if (clientUser.Roles != null)
        // {
        //     foreach (var role in clientUser.Roles)
        //         userIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
        // }


        var userPrincipal = new ClaimsPrincipal(userIdentity);

        if (httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            await SignOutAsync(cancellationToken: cancellationToken);

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
    }

    public async Task SignOutAsync(bool clearSession = false, CancellationToken cancellationToken = default)
    {
        cookieManager.Delete("CurrentUser_AuthenticationToken");
        sessionManager.Remove("CurrentUser");
        await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (clearSession)
            sessionManager.Clear();
    }

    public async Task<bool> HasClaimAsync(string type, string value, CancellationToken cancellationToken = default)
    {
        var currentUser = Get();
        if (currentUser == null)
            return false;

        var claims = new List<GetClientUserClaimsResult.Claim>();
        var claimSessionExpiry = sessionManager.Get<DateTime>("CurrentUser_Claims_Expiry");

        if (sessionManager.Exists("CurrentUser_Claims") && claimSessionExpiry > DateTime.Now)
        {
            claims = sessionManager.Get<List<GetClientUserClaimsResult.Claim>>("CurrentUser_Claims");
        }
        else
        {
            claims = (await mediator.Send(new GetClientUserClaimsQuery { Id = currentUser.Id }, cancellationToken)).Claims;
            sessionManager.Add("CurrentUser_Claims", claims);
            sessionManager.Add("CurrentUser_Claims_Expiry", DateTime.Now.AddMinutes(5));
        }

        return claims?.Any(x => x.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase) && x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase)) ?? false;

    }
}