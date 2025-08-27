using Blazored.LocalStorage;
using Microsoft.JSInterop;
using JSException = System.Runtime.InteropServices.JavaScript.JSException;

namespace ClearBudget.Infrastructure.Extensions;

public static class LocalStorageServiceExtensions
{

    public static async Task<T?> TryGetItemAsync<T>(this ILocalStorageService localStorageService, string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var item = await localStorageService.GetItemAsync<T>(key, cancellationToken);
            return item;
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException or JSException)
        {
            // JS interlop is not available.
            return default;
        }
    }

    public static async Task TrySetItemAsync<T>(this ILocalStorageService localStorageService, string key,
        T data, CancellationToken cancellationToken = default)
    {
        try
        {
            await localStorageService.SetItemAsync(key, data, cancellationToken);
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException or JSException)
        {
            // JS interlop is not available.
        }
    }

    public static async Task TryRemoveItemAsync(this ILocalStorageService localStorageService, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await localStorageService.RemoveItemAsync(key, cancellationToken);
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException or JSException)
        {
            // JS interlop is not available.
        }
    }
}
