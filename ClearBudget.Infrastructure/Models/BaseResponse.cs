using FluentValidation.Results;

namespace ClearBudget.Infrastructure.Models;

public class BaseResponse(List<RequestError>? errors = null)
{
    /// <summary>
    ///     Gets a value indicating whether the response is valid.
    /// </summary>
    public bool Success => Errors?.Count == 0;

    /// <summary>
    ///     Gets the list of error messages associated with the response.
    /// </summary>
    public List<RequestError> Errors { get; init; } = errors ?? [];

    public DateTime DateFinished { get; set; } = DateTime.Now;

    /// <summary>
    ///     Creates a successful response result.
    /// </summary>
    /// <returns>A successful response result.</returns>
    public static BaseResponse Succeeded()
    {
        return new BaseResponse();
    }

    /// <summary>
    ///     Creates a failed response result with the specified error messages.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed response result.</returns>
    public static BaseResponse Failed(string error)
    {
        return new BaseResponse([new RequestError("", error)]);
    }


    /// <summary>
    ///     Creates a failed response result with the specified error messages.
    /// </summary>
    /// <param name="error">The collection of error messages associated with the failure.</param>
    /// <returns>A failed response result.</returns>
    public static BaseResponse Failed(RequestError error)
    {
        return new BaseResponse([error]);
    }

    /// <summary>
    ///     Creates a failed response result with the specified error messages.
    /// </summary>
    /// <param name="errors">The collection of error messages associated with the failure.</param>
    /// <returns>A failed response result.</returns>
    public static BaseResponse Failed(IEnumerable<RequestError> errors)
    {
        return new BaseResponse(errors.ToList());
    }

    /// <summary>
    ///     Creates a failed response result with the specified error messages.
    /// </summary>
    /// <param name="errors">The collection of error messages associated with the failure.</param>
    /// <returns>A failed response result.</returns>
    public static BaseResponse Failed(IEnumerable<string> errors)
    {
        return new BaseResponse(errors.Select(x => new RequestError("", x)).ToList());
    }

    /// <summary>
    ///     Creates a failed response result with the specified error messages.
    /// </summary>
    /// <param name="errors">The collection of error messages associated with the failure.</param>
    /// <returns>A failed response result.</returns>
    public static BaseResponse Failed(IEnumerable<ValidationFailure> errors)
    {
        return new BaseResponse(errors.Select(x => new RequestError(x.PropertyName, x.ErrorMessage)).ToList());
    }
}

public class RequestError(string name, string errorMessage)
{
    public string PropertyName { get; set; } = name;
    public string ErrorMessage { get; set; } = errorMessage;
}