using Blazored.SessionStorage;
using Microsoft.JSInterop;
using JSException = System.Runtime.InteropServices.JavaScript.JSException;

namespace ClearBudget.Infrastructure.Extensions;

public static class SessionStorageServiceExtensions
{

    public static async Task<T?> TryGetItemAsync<T>(this ISessionStorageService sessionStorageService, string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var item = await sessionStorageService.GetItemAsync<T>(key, cancellationToken);
            return item;
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException or JSException)
        {
            // JS interlop is not available.
            return default;
        }
    }

    public static async Task TrySetItemAsync<T>(this ISessionStorageService sessionStorageService, string key,
        T data, CancellationToken cancellationToken = default)
    {
        try
        {
            await sessionStorageService.SetItemAsync(key, data, cancellationToken);
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException or JSException)
        {
            // JS interlop is not available.
        }
    }

    public static async Task TryRemoveItemAsync<T>(this ISessionStorageService sessionStorageService, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await sessionStorageService.RemoveItemAsync(key, cancellationToken);
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException or JSException)
        {
            // JS interlop is not available.
        }
    }
}
