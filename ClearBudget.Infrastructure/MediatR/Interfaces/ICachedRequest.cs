using ClearBudget.Infrastructure.Models;
using MediatR;

namespace ClearBudget.Infrastructure.MediatR.Interfaces;

public interface ICachedRequest<out TResponse> : IRequest<TResponse>
{
    public CacheKey GetCacheKey();
}