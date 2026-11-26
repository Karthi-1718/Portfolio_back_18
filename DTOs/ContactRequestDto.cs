using System.ComponentModel.DataAnnotations;

namespace DeveloperPortfolio.Api.DTOs;

public sealed class ContactRequestDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = "";

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; init; } = "";

    [Required, StringLength(160, MinimumLength = 2)]
    public string Subject { get; init; } = "";

    [Required, StringLength(4000, MinimumLength = 10)]
    public string Message { get; init; } = "";

    // Hidden honeypot field; legitimate visitors leave this empty.
    [StringLength(200)]
    public string Website { get; init; } = "";
}
