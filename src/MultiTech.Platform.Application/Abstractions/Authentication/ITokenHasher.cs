namespace MultiTech.Platform.Application.Abstractions.Authentication;

/// <summary>
/// Hashes high-entropy tokens for storage.
/// </summary>
public interface ITokenHasher
{
    /// <summary>
    /// Hashes the token.
    /// </summary>
    /// <param name="token">The token to hash.</param>
    /// <returns>The token hash.</returns>
    string Hash(string token);
}

