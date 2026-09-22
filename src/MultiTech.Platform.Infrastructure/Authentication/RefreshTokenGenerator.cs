using System.Security.Cryptography;
using MultiTech.Platform.Application.Abstractions.Authentication;

namespace MultiTech.Platform.Infrastructure.Authentication;

/// <summary>
/// Generates cryptographically secure refresh tokens.
/// </summary>
public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private const int TokenByteLength = 64;

    /// <inheritdoc />
    public string Generate()
    {
        Span<byte> bytes = stackalloc byte[TokenByteLength];
        RandomNumberGenerator.Fill(bytes);

        return Convert.ToBase64String(bytes);
    }
}

