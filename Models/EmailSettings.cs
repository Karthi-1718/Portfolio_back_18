namespace DeveloperPortfolio.Api.Models;

public sealed class EmailSettings
{
    public string ApiKey { get; init; } = "";
    public string From { get; init; } = "";
    public string To { get; init; } = "";
}
