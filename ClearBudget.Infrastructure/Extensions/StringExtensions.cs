namespace ClearBudget.Infrastructure.Extensions;

public static class StringExtensions
{
    public static string ToSafeString(this string? str)
    {
        return str ?? string.Empty;
    }
}
