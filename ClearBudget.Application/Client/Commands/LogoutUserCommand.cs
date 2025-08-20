using ClearBudget.Application.Services;
using ClearBudget.Infrastructure.Models;
using MediatR;

namespace ClearBudget.Application.Client.Commands;

public class LogoutUserCommand : IRequest<BaseResponse>
{
    internal class Handler(ICurrentUserService currentUserService) : IRequestHandler<LogoutUserCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(LogoutUserCommand request,
            CancellationToken cancellationToken = default)
        {
            await currentUserService.SignOutAsync(false, cancellationToken);
            return BaseResponse.Succeeded();
        }
    }
}