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

public class RegisterUserCommand : IRequest<BaseResponse>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public bool SignInUserAfterCreation { get; set; }

    internal class Handler(IDbContext context, ICurrentUserService currentUserService, IHashService hashService)
        : IRequestHandler<RegisterUserCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(RegisterUserCommand request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await new Validator().ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return BaseResponse.Failed(validationResult.Errors.Select(x => new RequestError(x.PropertyName, x.ErrorMessage)));

            var existingUser = await GetUserByEmailAsync(request, cancellationToken);
            if (existingUser != null)
                return BaseResponse.Failed("Email address already in use.");

            var clientUser = new ClientUser
            {
                Forename = request.FirstName,
                Surname = request.LastName,
                EmailAddress = request.EmailAddress,
                Password = hashService.Hash(request.Password),
                AuthenticationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                AuthenticationTokenExpiry = DateTime.UtcNow.AddDays(90),
                AuthenticationTokenGenerated = DateTime.UtcNow
            };

            context.ClientUsers.Add(clientUser);
            await context.SaveChangesAsync(cancellationToken);

            if (request.SignInUserAfterCreation)
                await currentUserService.SignInAsync(request.EmailAddress, request.Password, cancellationToken);

            return BaseResponse.Succeeded();
        }

        private async Task<ClientUser?> GetUserByEmailAsync(RegisterUserCommand request,
            CancellationToken cancellationToken = default)
        {
            return await (from cu in context.ClientUsers
                          where !cu.Deleted && cu.EmailAddress == request.EmailAddress
                          select cu).FirstOrDefaultAsync(cancellationToken);
        }
    }

    private class Validator : AbstractValidator<RegisterUserCommand>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("Email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirmation password is required.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}