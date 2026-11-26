using DeveloperPortfolio.Api.DTOs;
using DeveloperPortfolio.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DeveloperPortfolio.Api.Controllers;

[ApiController]
[Route("api/contact")]
public sealed class ContactController(IEmailService emailService, ILogger<ContactController> logger) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("contact")]
    public async Task<IActionResult> Send([FromBody] ContactRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { message = "Name, email, subject, and message are required." });

        if (!string.IsNullOrWhiteSpace(request.Website))
            return Ok(new { message = "Your message has been received." });

        try
        {
            await emailService.SendContactMessageAsync(
                request.Name.Trim(), request.Email.Trim(), request.Subject.Trim(), request.Message.Trim(), cancellationToken);
            return Ok(new { message = "Your message has been sent successfully." });
        }
        catch (EmailDeliveryException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { message = "We couldn't deliver your message right now. Please try again later." });
        }
        catch (InvalidOperationException exception)
        {
            logger.LogError(exception, "Contact email service is not configured.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "The contact service is temporarily unavailable." });
        }
    }
}
