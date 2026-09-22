namespace MultiTech.Platform.Application.Common.Results;

/// <summary>
/// Represents the outcome of an application operation with a value.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public sealed class Result<TValue> : Result
{
    private Result(TValue value)
        : base(true, null)
    {
        Value = value;
    }

    private Result(Error error)
        : base(false, error)
    {
    }

    /// <summary>
    /// Gets the result value when the operation succeeded.
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The successful result.</returns>
    public static Result<TValue> Success(TValue value) => new(value);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The failure error.</param>
    /// <returns>The failed result.</returns>
    public static new Result<TValue> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result<TValue>(error);
    }
}

