using ClearBudget.Infrastructure.MediatR.Interfaces;
using ClearBudget.Infrastructure.Services.Caching;
using MediatR;

namespace ClearBudget.Infrastructure.MediatR.Pipelines;

public class CachedRequestBehaviour<TRequest, TResponse>(ICacheManager cacheManager)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is ICachedRequest<TResponse> req)
        {
            var response = await cacheManager.GetOrCreateAsync(req.GetCacheKey().Title, () => next(),
                req.GetCacheKey().Duration, req.GetCacheKey().ExpirationType);
            if (response is not null) return response;
        }

        return await next();
    }
}