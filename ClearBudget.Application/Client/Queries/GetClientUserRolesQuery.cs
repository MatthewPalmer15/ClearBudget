using ClearBudget.Application.Client.Models;
using ClearBudget.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Client.Queries;

public class GetClientUserRolesQuery : IRequest<GetClientUserRolesResult>
{
    public Guid ClientUserId { get; set; }

    internal class Handler(IDbContext context) : IRequestHandler<GetClientUserRolesQuery, GetClientUserRolesResult>
    {
        public async Task<GetClientUserRolesResult> Handle(GetClientUserRolesQuery request, CancellationToken cancellationToken = default)
        {
            var roles = await (from cur in context.ClientUserRoles
                               join cr in context.ClientRoles on cur.ClientRoleId equals cr.Id
                               where cur.ClientUserId == request.ClientUserId
                               select new GetClientUserRolesResult.Role
                               {
                                   Title = cr.Name
                               }).ToListAsync(cancellationToken);

            return new GetClientUserRolesResult { Roles = roles };
        }
    }
}