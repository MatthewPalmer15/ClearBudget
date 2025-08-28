using ClearBudget.Application.Client.Queries;
using ClearBudget.Database;
using ClearBudget.Database.Entities.Client;
using ClearBudget.Infrastructure.Models;
using ClearBudget.Infrastructure.Services.Hash;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ClearBudget.Application.Client.Commands;

public class LoginUserCommand : IRequest<BaseResponse>
{
    public string EmailAddress { get; set; }
    public string Password { get; set; }

    internal class Handler(
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        IDbContext context,
        IHashService hashService) : IRequestHandler<LoginUserCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(LoginUserCommand request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await new Validator().ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return BaseResponse.Failed(validationResult.Errors.Select(x => new RequestError(x.PropertyName, x.ErrorMessage)));

            var user = await GetUserByEmailAsync(request, cancellationToken);
            if (user == null)
                return BaseResponse.Failed("User not found with that email.");

            if (!hashService.Verify(request.Password, user.Password))
                return BaseResponse.Failed("Incorrect password.");

            if (user.AuthenticationTokenExpiry <= DateTime.UtcNow)
            {
                user.AuthenticationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                user.AuthenticationTokenExpiry = DateTime.UtcNow.AddDays(90);
                user.AuthenticationTokenGenerated = DateTime.UtcNow;
            }

            context.ClientUsers.Update(user);
            await context.SaveChangesAsync(cancellationToken);

            if (httpContextAccessor?.HttpContext is null) return BaseResponse.Failed("Cannot log user in");

            var claimsResult = await mediator.Send(new GetClientUserClaimsQuery { ClientUserId = user.Id }, cancellationToken);
            var rolesResult = await mediator.Send(new GetClientUserRolesQuery { ClientUserId = user.Id }, cancellationToken);

            var userIdentity = new ClaimsIdentity("CurrentUser_Core", ClaimTypes.Name, ClaimTypes.Role);
            userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            userIdentity.AddClaim(new Claim(ClaimTypes.Name, user.EmailAddress));
            userIdentity.AddClaim(new Claim(ClaimTypes.Email, user.EmailAddress));
            userIdentity.AddClaim(new Claim("AuthenticationToken", user.AuthenticationToken));
            userIdentity.AddClaim(new Claim("FirstName", user.Forename));
            userIdentity.AddClaim(new Claim("LastName", user.Surname));

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

            await httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddHours(8),
                    IsPersistent = true,
                    AllowRefresh = true,
                    IssuedUtc = DateTime.UtcNow
                });
            return BaseResponse.Succeeded();
        }

        private async Task<ClientUser?> GetUserByEmailAsync(LoginUserCommand request,
            CancellationToken cancellationToken = default)
        {
            return await (from cu in context.ClientUsers
                          where !cu.Deleted && cu.EmailAddress == request.EmailAddress
                          select cu).FirstOrDefaultAsync(cancellationToken);
        }
    }

    private class Validator : AbstractValidator<LoginUserCommand>
    {
        public Validator()
        {
            RuleFor(x => x.EmailAddress)
                .NotNull().WithMessage("Email address is required.");

            RuleFor(x => x.Password)
                .NotNull().WithMessage("Password is required.");
        }
    }
}