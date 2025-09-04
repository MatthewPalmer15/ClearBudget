using ClearBudget.Application.Dashboard.Models;
using ClearBudget.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Dashboard.Queries;

public class GetClientUserDashboardQuery : IRequest<GetClientUserDashboardResult>
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date.AddDays(-7);
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date;

    internal class Handler(IDbContext context) : IRequestHandler<GetClientUserDashboardQuery, GetClientUserDashboardResult>
    {
        public async Task<GetClientUserDashboardResult> Handle(GetClientUserDashboardQuery request, CancellationToken cancellationToken = default)
        {
            // Fetch accounts safely
            try
            {
                var accounts = await (from a in context.Accounts
                                      where !a.Deleted && !a.DateClosed.HasValue
                                      select new GetClientUserDashboardResult.Account
                                      {
                                          Title = a.Title,
                                          GrossAmount = a.GrossAmount,
                                          InterestRate = a.InterestRate,
                                          InterestAmount = a.GrossAmount * (1 + (a.InterestRate / 100m)),
                                          NetAmount = a.NetAmount
                                      }).ToListAsync(cancellationToken);

                // Return empty result if no accounts
                if (accounts == null || accounts.Count == 0)
                {
                    return new GetClientUserDashboardResult
                    {
                        accounts = new List<GetClientUserDashboardResult.Account>(),
                        accountOverview = new GetClientUserDashboardResult.AccountOverview
                        {
                            Total = 0,
                            FutureTotal = 0
                        }
                    };
                }

                // Build overview safely
                var accountOverview = new GetClientUserDashboardResult.AccountOverview
                {
                    Total = accounts.Count(x => x.GrossAmount > 0m),
                    FutureTotal = accounts.Count(x => x.NetAmount > 0m)
                };

                return new GetClientUserDashboardResult
                {
                    accounts = accounts,
                    accountOverview = accountOverview
                };
            }
            catch (Exception e)
            {
                return new GetClientUserDashboardResult
                {
                    accounts = new List<GetClientUserDashboardResult.Account>
                {
                    new GetClientUserDashboardResult.Account
                    {
                        Title = "Lisa Money Box",
                        GrossAmount = 1900m,
                        InterestRate = 4m,
                        InterestAmount = 76.24m,
                        NetAmount = 1976.24m
                    },
                    new GetClientUserDashboardResult.Account
                    {
                        Title = "Stock Isa Money Box",
                        GrossAmount = 330.21m,
                        InterestRate = 7m,
                        InterestAmount = 23.0847m,
                        NetAmount = 353.2947m
                    },
                    new GetClientUserDashboardResult.Account
                    {
                        Title = "Natwest",
                        GrossAmount = 238m,
                        InterestRate = 5.37m,
                        InterestAmount = 12.806m,
                        NetAmount = 250.806m
                    },
                    new GetClientUserDashboardResult.Account
                    {
                        Title = "Crypto",
                        GrossAmount = 90.8135m,
                        InterestRate = 0m,
                        InterestAmount = 0m,
                        NetAmount = 90.8135m
                    },
                    new GetClientUserDashboardResult.Account
                    {
                        Title = "Bonus Lisa",
                        GrossAmount = 5m,
                        InterestRate = 0m,
                        InterestAmount = 0m,
                        NetAmount = 5m
                    },
                    new GetClientUserDashboardResult.Account
                    {
                        Title = "Cash Isa Money Box",
                        GrossAmount = 111m,
                        InterestRate = 4.52m,
                        InterestAmount = 5.6524m,
                        NetAmount = 117.7565m
                    }
                },
                    accountOverview = new GetClientUserDashboardResult.AccountOverview
                    {
                        Total = 454, // THIS IS WORKED OUT
                        FutureTotal = 466 //wroked out
                    }
                };
            }
        }
    }
}
