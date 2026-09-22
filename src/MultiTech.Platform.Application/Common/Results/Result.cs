namespace MultiTech.Platform.Application.Common.Results;

/// <summary>
/// Represents the outcome of an application operation.
/// </summary>
public class Result
{
    private protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error when the operation failed.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>The successful result.</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The failure error.</param>
    /// <returns>The failed result.</returns>
    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result(false, error);
    }
}

