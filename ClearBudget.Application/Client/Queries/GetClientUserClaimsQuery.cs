using ClearBudget.Application.Client.Models;
using ClearBudget.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Client.Queries;

public class GetClientUserClaimsQuery : IRequest<GetClientUserClaimsResult>
{
    public Guid ClientUserId { get; set; }

    internal class Handler(IDbContext context) : IRequestHandler<GetClientUserClaimsQuery, GetClientUserClaimsResult>
    {
        public async Task<GetClientUserClaimsResult> Handle(GetClientUserClaimsQuery request, CancellationToken cancellationToken = default)
        {
            var claims = new List<GetClientUserClaimsResult.Claim>();

            var userClaims = await (from cur in context.ClientUserClaims
                                    join cu in context.ClientUsers on cur.ClientUserId equals cu.Id
                                    where !cur.Deleted
                                          && !cu.Deleted
                                          && cu.Id == request.ClientUserId
                                    select new GetClientUserClaimsResult.Claim
                                    {
                                        Type = cur.Type,
                                        Value = cur.Value,
                                        FromRole = false
                                    }).ToListAsync(cancellationToken);

            claims.AddRange(userClaims);
            var existingClaimTypes = new HashSet<string>(userClaims.Select(x => x.Type));


            var roleClaims = await (from crc in context.ClientRoleClaims
                                    join cr in context.ClientRoles on crc.ClientRoleId equals cr.Id
                                    join cur in context.ClientUserRoles on cr.Id equals cur.ClientRoleId
                                    join cu in context.ClientUsers on cur.ClientUserId equals cu.Id
                                    where !crc.Deleted
                                          && !cr.Deleted
                                          && !cur.Deleted
                                          && !cu.Deleted
                                          && cu.Id == request.ClientUserId
                                          && !existingClaimTypes.Contains(crc.Type)
                                    select new GetClientUserClaimsResult.Claim
                                    {
                                        Type = crc.Type,
                                        Value = crc.Value,
                                        FromRole = true
                                    }).ToListAsync(cancellationToken);

            claims.AddRange(roleClaims);

            return new GetClientUserClaimsResult { Claims = claims };
        }
    }
}