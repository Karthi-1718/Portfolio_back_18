using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DeveloperPortfolio.Api.Models;
using Microsoft.Extensions.Options;

namespace DeveloperPortfolio.Api.Services;

public sealed class EmailService(HttpClient http, IOptions<EmailSettings> options, ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendContactMessageAsync(string name, string email, string subject, string message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey) || string.IsNullOrWhiteSpace(_settings.From) || string.IsNullOrWhiteSpace(_settings.To))
            throw new InvalidOperationException("Email delivery is not configured.");

        using var request = new HttpRequestMessage(HttpMethod.Post, "emails");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        request.Content = JsonContent.Create(new
        {
            from = _settings.From,
            to = new[] { _settings.To },
            reply_to = email,
            subject = $"Portfolio contact: {subject}",
            text = $"Name: {name}\nEmail: {email}\n\n{message}"
        });

        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var providerResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Email provider returned {StatusCode}: {ProviderResponse}", (int)response.StatusCode, providerResponse);
            throw new EmailDeliveryException();
        }
    }
}

public sealed class EmailDeliveryException : Exception;
