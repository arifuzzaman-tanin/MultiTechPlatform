using System.Security.Cryptography;
using System.Text;
using MultiTech.Platform.Application.Abstractions.Authentication;

namespace MultiTech.Platform.Infrastructure.Authentication;

/// <summary>
/// Hashes refresh tokens for lookup and storage.
/// </summary>
public sealed class TokenHasher : ITokenHasher
{
    /// <inheritdoc />
    public string Hash(string token)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}

