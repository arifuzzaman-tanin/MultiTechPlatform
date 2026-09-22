using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MultiTech.Platform.Application.Common.Results;

namespace MultiTech.Platform.Api.Http;

/// <summary>
/// Maps application results to HTTP responses.
/// </summary>
public static class HttpResults
{
    /// <summary>
    /// Converts an error to a problem response.
    /// </summary>
    /// <param name="error">The application error.</param>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The problem response.</returns>
    public static IResult Problem(Error error, HttpContext httpContext)
    {
        int statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(new ProblemDetails
        {
            Title = ReasonPhrases.GetReasonPhrase(statusCode),
            Detail = error.Message,
            Status = statusCode,
            Extensions =
            {
                ["code"] = error.Code,
                ["traceId"] = httpContext.TraceIdentifier
            }
        });
    }
}
