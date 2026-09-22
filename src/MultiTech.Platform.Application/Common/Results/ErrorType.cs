namespace MultiTech.Platform.Application.Common.Results;

/// <summary>
/// Identifies the category of an application error.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Represents an unexpected or general failure.
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Represents invalid input.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Represents a missing resource.
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Represents a conflict with current state.
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// Represents missing or invalid authentication.
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// Represents denied authorization.
    /// </summary>
    Forbidden = 5
}

