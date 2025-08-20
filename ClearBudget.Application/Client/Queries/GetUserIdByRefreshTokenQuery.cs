using ClearBudget.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Client.Queries;

public class GetUserIdByRefreshTokenQuery : IRequest<Guid?>
{
    public string AuthenticationToken { get; set; }

    internal class Handler(IDbContext context) : IRequestHandler<GetUserIdByRefreshTokenQuery, Guid?>
    {
        public async Task<Guid?> Handle(GetUserIdByRefreshTokenQuery request,
            CancellationToken cancellationToken = default)
        {
            return await (from cu in context.ClientUsers
                          where !cu.Deleted
                                && cu.AuthenticationToken == request.AuthenticationToken
                                && cu.AuthenticationTokenExpiry.HasValue
                                && cu.AuthenticationTokenExpiry.Value > DateTime.Now
                          select cu.Id).FirstOrDefaultAsync(cancellationToken);
        }
    }
}