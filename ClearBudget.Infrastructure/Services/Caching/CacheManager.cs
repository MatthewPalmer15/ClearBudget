// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ClearBudget.Infrastructure.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ClearBudget.Infrastructure.Services.Caching;

public interface ICacheManager
{
    /// <summary>
    ///     Checks if a value exists in the cache for the specified key.
    /// </summary>
    /// <param name="key">The key to check for existence in the cache.</param>
    /// <returns>
    ///     True if a value exists in the cache for the specified key; otherwise, false.
    /// </returns>
    public bool Exists(string key);

    /// <summary>
    ///     Retrieves a value from the cache with the specified key, if it exists.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve from the cache.</typeparam>
    /// <param name="key">The key associated with the value in the cache.</param>
    /// <returns>
    ///     The value associated with the specified key if found; otherwise, the default value of type
    ///     <typeparamref name="T" />.
    /// </returns>
    public T? Get<T>(string key);

    /// <summary>
    ///     Retrieves a value from the cache with the specified key, or creates it if it does not exist.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve or create in the cache.</typeparam>
    /// <param name="key">The key associated with the value in the cache.</param>
    /// <param name="func">A function to generate the value if it does not exist in the cache.</param>
    /// <param name="expiration">
    ///     The time span after which the value will expire and be removed from the cache.
    ///     If not specified, the default expiration is 15 minutes.
    /// </param>
    /// <param name="expirationType">Specifies the type of cache expiration.</param>
    /// <returns>
    ///     The value associated with the specified key if found; otherwise, the result of invoking the provided function to
    ///     generate the value.
    /// </returns>
    /// <remarks>
    ///     If the value does not exist in the cache, it will be added and will expire after the specified duration.
    /// </remarks>
    public T? GetOrCreate<T>(string key, Func<T> func, TimeSpan? expiration = null,
        CacheExpiration expirationType = CacheExpiration.Absolute);

    /// <summary>
    ///     Asynchronously retrieves a value from the cache with the specified key, or creates it if it does not exist.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve or create in the cache.</typeparam>
    /// <param name="key">The key associated with the value in the cache.</param>
    /// <param name="func">A function to asynchronously generate the value if it does not exist in the cache.</param>
    /// <param name="expiration">
    ///     The time span after which the value will expire and be removed from the cache.
    ///     If not specified, the default expiration is 15 minutes.
    /// </param>
    /// <param name="expirationType">Specifies the type of cache expiration.</param>
    /// <returns>
    ///     A task representing the asynchronous operation that, upon completion, returns the value associated with the
    ///     specified key if found;
    ///     otherwise, returns the result of invoking the provided asynchronous function to generate the value.
    /// </returns>
    /// <remarks>
    ///     If the value does not exist in the cache, it will be added and will expire after the specified duration.
    /// </remarks>
    public Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null,
        CacheExpiration expirationType = CacheExpiration.Absolute);

    /// <summary>
    ///     Asynchronously adds a value to the cache with the specified key and expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to add to the cache.</typeparam>
    /// <param name="key">The key under which to store the value in the cache.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <param name="expiration">
    ///     The time span after which the value will expire and be removed from the cache.
    ///     If not specified, the default expiration is 15 minutes.
    /// </param>
    /// <param name="expirationType">Specifies the type of cache expiration.</param>
    /// <returns>
    ///     A task representing the asynchronous operation that, upon completion, returns the value added to the cache.
    /// </returns>
    /// <remarks>
    ///     The added value will expire and be removed from the cache after the specified duration.
    /// </remarks>
    public T Set<T>(string key, T value, TimeSpan? expiration = null,
        CacheExpiration expirationType = CacheExpiration.Absolute);

    /// <summary>
    ///     Removes an item from the cache with the specified key.
    /// </summary>
    /// <param name="key">The key associated with the item to be removed from the cache.</param>
    /// <remarks>
    ///     If the specified key is found in the cache, the associated item will be removed.
    /// </remarks>
    public void Remove(string key);

    /// <summary>
    ///     Clears the cache, removing all items.
    /// </summary>
    /// <remarks>
    ///     This operation cancels any ongoing cache reset process and initializes a new cancellation token for future
    ///     operations.
    /// </remarks>
    public void Clear();
}

internal class CacheManager(IMemoryCache memoryCache) : ICacheManager
{
    private readonly IMemoryCache _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    private readonly HashSet<string> _cacheKeys = [];

    private CancellationTokenSource _resetCacheToken = new();

    /// <inheritdoc />
    public bool Exists(string key)
    {
        return _cache.TryGetValue(key, out _);
    }

    /// <inheritdoc />
    public T? Get<T>(string key)
    {
        return Exists(key) ? _cache.Get<T>(key) : default;
    }

    /// <inheritdoc />
    public T? GetOrCreate<T>(string key, Func<T> func, TimeSpan? expiration = null,
        CacheExpiration expirationType = CacheExpiration.Absolute)
    {
        var item = _cache.GetOrCreate(key, entry =>
        {
            SetCacheExpiration(entry, expirationType, expiration ?? TimeSpan.FromMinutes(15));
            return func();
        });
        _cacheKeys.Add(key);
        return item;
    }

    /// <inheritdoc />
    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null,
        CacheExpiration expirationType = CacheExpiration.Absolute)
    {
        var item = await _cache.GetOrCreateAsync(key, entry =>
        {
            SetCacheExpiration(entry, expirationType, expiration ?? TimeSpan.FromMinutes(15));

            return func();
        });
        _cacheKeys.Add(key);
        return item;
    }

    /// <inheritdoc />
    public T Set<T>(string key, T value, TimeSpan? expiration = null,
        CacheExpiration expirationType = CacheExpiration.Absolute)
    {
        var item = _cache.Set(key, value, SetCacheExpiration(expirationType, expiration ?? TimeSpan.FromMinutes(15)));
        _cacheKeys.Add(key);
        return item;
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        _cache.Remove(key);
        _cacheKeys.Remove(key);
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (_resetCacheToken != null && _resetCacheToken is
                { IsCancellationRequested: false, Token.CanBeCanceled: true })
        {
            _resetCacheToken.Cancel();
            _resetCacheToken.Dispose();
        }

        _resetCacheToken = new CancellationTokenSource();

        foreach (var cacheKey in _cacheKeys)
        {
            _cache.Remove(cacheKey);
            _cacheKeys.Remove(cacheKey);
        }
    }

    private void SetCacheExpiration(ICacheEntry entry, CacheExpiration type, TimeSpan expiration)
    {
        switch (type)
        {
            case CacheExpiration.Absolute:
                entry.AbsoluteExpirationRelativeToNow = expiration;
                break;

            case CacheExpiration.Sliding:
                entry.SlidingExpiration = expiration;
                break;
        }

        entry.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
    }

    private static MemoryCacheEntryOptions SetCacheExpiration(CacheExpiration type, TimeSpan expiration)
    {
        MemoryCacheEntryOptions? options = new();
        switch (type)
        {
            case CacheExpiration.Absolute:
                options.AbsoluteExpirationRelativeToNow = expiration;
                break;

            case CacheExpiration.Sliding:
                options.SlidingExpiration = expiration;
                break;
        }

        return options;
    }
}