using ClearBudget.Application.Dashboard.Models;
using ClearBudget.Application.Finance.Models;
using ClearBudget.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Dashboard.Queries;

public class GetClientFinanceQuery : IRequest<GetClientFinanceResult>
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date.AddDays(-7);
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date;

    //internal class Handler(IDbContext context) : IRequestHandler<GetClientFinanceQuery, GetClientFinanceResult>
    //{
    //    //public async Task<GetClientFinanceResult> Handle(GetClientFinanceQuery request, CancellationToken cancellationToken = default)
    //    //{
    //    //    ret
    //    //}
    //}
}
