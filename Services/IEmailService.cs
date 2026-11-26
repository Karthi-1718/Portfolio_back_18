namespace DeveloperPortfolio.Api.Services;

public interface IEmailService
{
    Task SendContactMessageAsync(string name, string email, string subject, string message, CancellationToken cancellationToken);
}
