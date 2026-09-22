using System.Net.Mail;
using MultiTech.Platform.Application.Common.Results;

namespace MultiTech.Platform.Application.Features.Authentication;

/// <summary>
/// Provides authentication input validation helpers.
/// </summary>
internal static class AuthValidation
{
    private const int MaximumEmailLength = 320;
    private const int MinimumNameLength = 2;
    private const int MaximumNameLength = 150;
    private const int MinimumCompanyLength = 2;
    private const int MaximumCompanyLength = 200;
    private const int MinimumPasswordLength = 8;
    private const int MaximumPasswordLength = 128;

    /// <summary>
    /// Normalizes an email address for lookup.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>The normalized email address.</returns>
    public static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    /// <summary>
    /// Validates registration fields.
    /// </summary>
    /// <param name="name">The display name.</param>
    /// <param name="email">The email address.</param>
    /// <param name="company">The company name.</param>
    /// <param name="password">The password.</param>
    /// <returns>An error when invalid; otherwise <see langword="null"/>.</returns>
    public static Error? ValidateRegistration(
        string? name,
        string? email,
        string? company,
        string? password)
    {
        Error? nameError = ValidateRequiredLength(
            name,
            "Auth.InvalidName",
            "Name is required and must be between 2 and 150 characters.",
            MinimumNameLength,
            MaximumNameLength);

        if (nameError is not null)
        {
            return nameError;
        }

        Error? emailError = ValidateEmail(email);
        if (emailError is not null)
        {
            return emailError;
        }

        Error? companyError = ValidateRequiredLength(
            company,
            "Auth.InvalidCompany",
            "Company is required and must be between 2 and 200 characters.",
            MinimumCompanyLength,
            MaximumCompanyLength);

        if (companyError is not null)
        {
            return companyError;
        }

        return ValidatePassword(password, true);
    }

    /// <summary>
    /// Validates login fields.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="password">The password.</param>
    /// <returns>An error when invalid; otherwise <see langword="null"/>.</returns>
    public static Error? ValidateLogin(string? email, string? password)
    {
        Error? emailError = ValidateEmail(email);
        if (emailError is not null)
        {
            return emailError;
        }

        return ValidatePassword(password, false);
    }

    private static Error? ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Trim().Length > MaximumEmailLength)
        {
            return Error.Validation(
                "Auth.InvalidEmail",
                "Email is required and must be a valid email address.");
        }

        try
        {
            _ = new MailAddress(email.Trim());
            return null;
        }
        catch (FormatException)
        {
            return Error.Validation(
                "Auth.InvalidEmail",
                "Email is required and must be a valid email address.");
        }
    }

    private static Error? ValidatePassword(string? password, bool requireMinimumLength)
    {
        if (string.IsNullOrEmpty(password) || password.Length > MaximumPasswordLength)
        {
            return Error.Validation(
                "Auth.InvalidPassword",
                "Password is required and must be no more than 128 characters.");
        }

        if (requireMinimumLength && password.Length < MinimumPasswordLength)
        {
            return Error.Validation(
                "Auth.InvalidPassword",
                "Password must be between 8 and 128 characters.");
        }

        return null;
    }

    private static Error? ValidateRequiredLength(
        string? value,
        string code,
        string message,
        int minimumLength,
        int maximumLength)
    {
        int length = value?.Trim().Length ?? 0;
        if (length < minimumLength || length > maximumLength)
        {
            return Error.Validation(code, message);
        }

        return null;
    }
}

