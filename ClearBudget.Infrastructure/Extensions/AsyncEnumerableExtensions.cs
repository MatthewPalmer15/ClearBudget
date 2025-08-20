namespace ClearBudget.Infrastructure.Extensions;

public static class AsyncEnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items,
        CancellationToken cancellationToken = default)
    {
        List<T>? results = [];
        await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false)) results.Add(item);

        return results;
    }
}