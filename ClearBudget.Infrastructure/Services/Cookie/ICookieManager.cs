using Microsoft.AspNetCore.Http;

namespace ClearBudget.Infrastructure.Services.Cookie;

public interface ICookieManager
{
    /// <summary>
    ///     Gets the cookies from the HTTP request.
    /// </summary>
    /// <value>
    ///     The cookies as an IRequestCookieCollection.
    /// </value>
    public IRequestCookieCollection Cookies { get; }

    /// <summary>
    ///     Retrieves the total number of cookies in the HTTP request.
    /// </summary>
    public int Count { get; }

    /// <summary>
    ///     Checks if a cookie with the specified key exists in the HTTP request.
    /// </summary>
    /// <param name="key">The key of the cookie to check for existence.</param>
    /// <returns>
    ///     True if a cookie with the specified key exists in the HTTP request; otherwise, false.
    /// </returns>
    public bool Exists(string key);

    /// <summary>
    ///     Retrieves the value of the specified cookie from the HTTP request.
    /// </summary>
    /// <param name="key">The key of the cookie to retrieve.</param>
    /// <returns>
    ///     The value of the specified cookie if found; otherwise, null.
    /// </returns>
    public string? Get(string key);

    /// <summary>
    ///     Retrieves a cookie from the HTTP request and attempts to convert its value to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the cookie value to.</typeparam>
    /// <param name="key">The key of the cookie.</param>
    /// <returns>
    ///     The value of the cookie as the specified type, or default value of the type if the cookie does not exist or
    ///     cannot be converted to the specified type.
    /// </returns>
    public T? Get<T>(string key);

    /// <summary>
    ///     Adds a cookie to the HTTP response with the specified key, value, and default expiration time of 60 minutes.
    /// </summary>
    /// <param name="key">The key of the cookie to add.</param>
    /// <param name="value">The value of the cookie.</param>
    /// <remarks>
    ///     The cookie will have SameSite mode set to None, HttpOnly set to false, and Secure set to true.
    /// </remarks>
    public void Add(string key, string value);

    /// <summary>
    ///     Adds a cookie to the HTTP response with the specified key, value, and expiration time.
    /// </summary>
    /// <param name="key">The key of the cookie to add.</param>
    /// <param name="value">The value of the cookie.</param>
    /// <param name="expires">The time span after which the cookie will expire.</param>
    /// <param name="mode">The SameSite mode of the cookie. Default is None.</param>
    /// <param name="httpOnly">A value indicating whether the cookie is accessible by client-side script. Default is false.</param>
    /// <param name="secure">
    ///     A value indicating whether to transmit the cookie using Secure Sockets Layer (SSL). Default is
    ///     true.
    /// </param>
    /// <remarks>
    ///     The cookie will have SameSite mode set to None, HttpOnly set to false, and Secure set to true by default.
    /// </remarks>
    public void Add(string key, string value, TimeSpan expires, SameSiteMode mode = SameSiteMode.None,
        bool httpOnly = false, bool secure = true);

    /// <summary>
    ///     Deletes the cookie with the specified key from the HTTP response.
    /// </summary>
    /// <param name="key">The key of the cookie to delete.</param>
    /// <remarks>
    ///     If a cookie with the specified key exists in the HTTP response, it will be deleted.
    /// </remarks>
    public void Delete(string key);
}