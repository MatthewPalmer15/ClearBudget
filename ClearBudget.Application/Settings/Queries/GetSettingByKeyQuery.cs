using ClearBudget.Application.Settings.Models;
using ClearBudget.Database;
using ClearBudget.Infrastructure.Extensions;
using ClearBudget.Infrastructure.MediatR.Interfaces;
using ClearBudget.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Settings.Queries;

public class GetSettingByKeyQuery : ICachedRequest<GetSettingByKeyResult?>
{
    public string Key { get; set; }
    public CacheKey GetCacheKey() => new($"GetSettingByKeyQuery:{Key.ToSafeString().ToLower()}", TimeSpan.FromMinutes(1));

    internal class Handler(IDbContext context) : IRequestHandler<GetSettingByKeyQuery, GetSettingByKeyResult?>
    {
        public async Task<GetSettingByKeyResult?> Handle(GetSettingByKeyQuery request, CancellationToken cancellationToken = default)
        {
            return await (from s in context.Settings
                          where s.Key == request.Key
                          select new GetSettingByKeyResult
                          {
                              Id = s.Id,
                              Key = s.Key,
                              Value = s.Value
                          }).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }
    }
}