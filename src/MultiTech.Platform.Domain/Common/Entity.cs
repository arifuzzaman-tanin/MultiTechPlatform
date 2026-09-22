namespace MultiTech.Platform.Domain.Common;

/// <summary>
/// Represents a domain entity with a stable identity.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Gets the entity identifier.
    /// </summary>
    public Guid Id { get; protected init; }
}

