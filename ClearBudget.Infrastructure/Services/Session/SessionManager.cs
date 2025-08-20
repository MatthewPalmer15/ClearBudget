using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace ClearBudget.Infrastructure.Services.Session;

public interface ISessionManager
{
    public bool Exists(string key);
    public object? Get(string key);
    public T? Get<T>(string key);
    public void Add<T>(string key, T value);
    public void Remove(string key);
    public void Clear();
}

internal class SessionManager(IHttpContextAccessor httpContextAccessor) : ISessionManager
{
    private readonly HttpContext _httpContext =
        httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public bool Exists(string key)
    {
        return _httpContext.Session.Keys.Contains(key);
    }

    public object? Get(string key)
    {
        if (!_httpContext.Session.TryGetValue(key, out var bytes))
            return default;

        var json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject(json,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }

    public T? Get<T>(string key)
    {
        if (!_httpContext.Session.TryGetValue(key, out var bytes))
            return default;

        var json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<T>(json,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }

    public void Add<T>(string key, T value)
    {
        var json = JsonConvert.SerializeObject(value,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        var bytes = Encoding.UTF8.GetBytes(json);
        _httpContext.Session.Set(key, bytes);
    }

    public void Remove(string key)
    {
        _httpContext.Session.Remove(key);
    }

    public void Clear()
    {
        _httpContext.Session.Clear();
    }
}