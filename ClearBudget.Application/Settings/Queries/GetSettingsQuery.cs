using ClearBudget.Application.Settings.Models;
using ClearBudget.Database;
using ClearBudget.Infrastructure.MediatR.Interfaces;
using ClearBudget.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Settings.Queries;

public class GetSettingsQuery : ICachedRequest<GetSettingsResult>
{
    public CacheKey GetCacheKey() => new("GetSettingsQuery", TimeSpan.FromMinutes(1));

    internal class Handler(IDbContext context) : IRequestHandler<GetSettingsQuery, GetSettingsResult>
    {
        public async Task<GetSettingsResult> Handle(GetSettingsQuery request, CancellationToken cancellationToken = default)
        {
            var settings = await (from s in context.Settings
                                  select new GetSettingsResult.Setting
                                  {
                                      Id = s.Id,
                                      Key = s.Key,
                                      Value = s.Value
                                  }).AsNoTracking().ToListAsync(cancellationToken);

            return new GetSettingsResult { Settings = settings };
        }
    }
}