namespace MultiTech.Platform.Application.Abstractions.Persistence;

/// <summary>
/// Represents a duplicate normalized email persistence conflict.
/// </summary>
public sealed class DuplicateEmailException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateEmailException"/> class.
    /// </summary>
    public DuplicateEmailException()
        : base("A user with the same normalized email already exists.")
    {
    }
}

