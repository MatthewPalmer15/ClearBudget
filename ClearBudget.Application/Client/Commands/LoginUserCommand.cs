using ClearBudget.Application.Services;
using ClearBudget.Database;
using ClearBudget.Database.Entities.Client;
using ClearBudget.Infrastructure.Models;
using ClearBudget.Infrastructure.Services.Hash;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ClearBudget.Application.Client.Commands;

public class LoginUserCommand : IRequest<BaseResponse>
{
    public string EmailAddress { get; set; }
    public string Password { get; set; }

    internal class Handler(
        ICurrentUserService currentUserService,
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

            await currentUserService.SignInAsync(user, cancellationToken);
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