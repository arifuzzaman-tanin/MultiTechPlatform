namespace MultiTech.Platform.Application.Common.Results;

/// <summary>
/// Describes an expected application error.
/// </summary>
/// <param name="Code">The stable error code.</param>
/// <param name="Message">The safe error message.</param>
/// <param name="Type">The error category.</param>
public sealed record Error(
    string Code,
    string Message,
    ErrorType Type)
{
    /// <summary>
    /// Creates a validation error.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The safe error message.</param>
    /// <returns>The validation error.</returns>
    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The safe error message.</param>
    /// <returns>The conflict error.</returns>
    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Creates an unauthorized error.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The safe error message.</param>
    /// <returns>The unauthorized error.</returns>
    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// Creates a not-found error.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The safe error message.</param>
    /// <returns>The not-found error.</returns>
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);
}

