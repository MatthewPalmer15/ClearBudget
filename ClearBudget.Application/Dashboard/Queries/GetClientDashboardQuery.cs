using ClearBudget.Application.Dashboard.Models;
using ClearBudget.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Dashboard.Queries;

public class GetClientUserDashboardQuery : IRequest<GetClientUserDashboardResult>
{
    public Guid ClientUserId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date.AddDays(-7);
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date;

    internal class Handler(IDbContext context) : IRequestHandler<GetClientUserDashboardQuery, GetClientUserDashboardResult>
    {
        public async Task<GetClientUserDashboardResult> Handle(GetClientUserDashboardQuery request, CancellationToken cancellationToken = default)
        {
            var transactions = await (from a in context.Accounts
                                      join at in context.AccountTransactions on a.Id equals at.AccountId
                                      where !a.Deleted
                                            && !a.DateClosed.HasValue
                                            && !at.Deleted
                                            && at.TransactionDate >= request.StartDate
                                            && at.TransactionDate < request.EndDate
                                            && a.ClientUserId == request.ClientUserId
                                      select new GetClientUserDashboardResult.Transaction
                                      {

                                      }).ToListAsync(cancellationToken);

            return new GetClientUserDashboardResult { Transactions = transactions };
        }
    }
}
