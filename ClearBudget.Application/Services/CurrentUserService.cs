using Blazored.LocalStorage;
using ClearBudget.Application.Client.Queries;
using ClearBudget.Database.Entities.Client;
using ClearBudget.Infrastructure.Extensions;
using ClearBudget.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Security.Claims;


namespace ClearBudget.Application.Services;

public sealed record CurrentUserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public bool IsAuthenticated { get; set; }
}

public interface ICurrentUserService
{
    Task<CurrentUserDto> GetAsync(CancellationToken cancellationToken = default);
    Task<bool> SignInAsync(ClientUser clientUser, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
    Task<bool> HasClaimAsync(string type, string value, CancellationToken cancellationToken = default);
    Task<bool> IsInRoleAsync(string roleName, CancellationToken cancellationToken = default);
}

public class CurrentUserService(IMediator mediator,
    IHttpContextAccessor httpContextAccessor,
    ILocalStorageService localStorageService,
    HttpClient httpClient) : ICurrentUserService
{
    public async Task<CurrentUserDto> GetAsync(CancellationToken cancellationToken = default)
    {
        CurrentUserDto? user;
        if (OperatingSystem.IsBrowser())
        {
            user = await localStorageService.TryGetItemAsync<CurrentUserDto>("CurrentUser:v1", cancellationToken);
            if (user != null) return user;

            var httpResponse = await httpClient.GetAsync("/api/auth/get", cancellationToken);
            if (httpResponse.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden)
            {
                await localStorageService.TryRemoveItemAsync("CurrentUser:v1", cancellationToken);
                return new CurrentUserDto { IsAuthenticated = false };
            }

            if (!httpResponse.IsSuccessStatusCode) return new CurrentUserDto { IsAuthenticated = false }; ;

            user = await httpResponse.Content.ReadFromJsonAsync<CurrentUserDto?>(cancellationToken);
            if (user == null)
                return new CurrentUserDto { IsAuthenticated = false };

            await localStorageService.TrySetItemAsync("CurrentUser:v1", user, cancellationToken);
            return user;
        }

        var userPrincipal = httpContextAccessor.HttpContext.User;
        if (userPrincipal is null || !(userPrincipal.Identity?.IsAuthenticated ?? false)) return new CurrentUserDto { IsAuthenticated = false }; ;

        var claimNameIdentifier = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(claimNameIdentifier, out var clientUserId))
            return new CurrentUserDto { IsAuthenticated = false }; ;

        return new CurrentUserDto
        {
            Id = clientUserId,
            FirstName = userPrincipal.FindFirst("FirstName")?.Value ?? "",
            LastName = userPrincipal.FindFirst("LastName")?.Value ?? "",
            EmailAddress = userPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? userPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? "",
            IsAuthenticated = true
        };
    }

    public async Task<bool> SignInAsync(ClientUser clientUser, CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsBrowser())
        {
            var currentUser = await localStorageService.TryGetItemAsync<ClientUser>("CurrentUser", cancellationToken);
            if (currentUser != null) return false; // Already logged in.

            var httpResponse = await httpClient.PostAsJsonAsync("/api/auth/login", new
            {
                clientUser.EmailAddress,
                clientUser.AuthenticationToken
            }, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode) return false;

            var result = await httpResponse.Content.ReadFromJsonAsync<BaseResponse>(cancellationToken);
            return result?.Success ?? false;
        }

        if (httpContextAccessor?.HttpContext is null) return false;
        if (clientUser == null || string.IsNullOrWhiteSpace(clientUser.AuthenticationToken)) return false;

        var claimsResult = await mediator.Send(new GetClientUserClaimsQuery { ClientUserId = clientUser.Id }, cancellationToken);
        var rolesResult = await mediator.Send(new GetClientUserRolesQuery { ClientUserId = clientUser.Id }, cancellationToken);

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
            await SignOutAsync(cancellationToken);

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

        return true;
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsBrowser())
        {
            await localStorageService.TryRemoveItemAsync("CurrentUser", cancellationToken);
            await httpClient.PostAsync("/api/auth/logout", null, cancellationToken);
            return;
        }

        await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<bool> HasClaimAsync(string type, string value, CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsBrowser())
        {
            var httpResponse = await httpClient.PostAsJsonAsync("/api/auth/hasclaim", new { type, value }, cancellationToken);
            if (httpResponse.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden)
            {
                await localStorageService.TryRemoveItemAsync("CurrentUser", cancellationToken);
                return false;
            }

            if (!httpResponse.IsSuccessStatusCode) return false;

            var response = await httpResponse.Content.ReadFromJsonAsync<bool>(cancellationToken);
            return response;
        }

        var userPrincipal = httpContextAccessor.HttpContext.User;
        if (!(userPrincipal.Identity?.IsAuthenticated ?? false)) return false;

        return userPrincipal.HasClaim(type, value);
    }

    public async Task<bool> IsInRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsBrowser())
        {
            var httpResponse = await httpClient.PostAsJsonAsync("/api/auth/isinrole", new { roleName }, cancellationToken);
            if (httpResponse.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden)
            {
                await localStorageService.TryRemoveItemAsync("CurrentUser", cancellationToken);
                return false;
            }

            if (!httpResponse.IsSuccessStatusCode) return false;

            var response = await httpResponse.Content.ReadFromJsonAsync<bool>(cancellationToken);
            return response;
        }

        var userPrincipal = httpContextAccessor.HttpContext.User;
        if (!(userPrincipal.Identity?.IsAuthenticated ?? false)) return false;

        return userPrincipal.IsInRole(roleName);
    }
}