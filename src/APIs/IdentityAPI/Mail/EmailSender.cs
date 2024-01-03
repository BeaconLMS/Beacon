using Azure;
using Azure.Communication.Email;
using Azure.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace IdentityAPI.Mail;

public class EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> settings) : IEmailSender
{
    private readonly ILogger<EmailSender> _logger = logger;
    private readonly EmailSettings _settings = settings.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new EmailMessage(_settings.MailFrom, email, new EmailContent(subject)
        {
            Html = htmlMessage
        });

        await SendEmailAsync(message);
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        try
        {
            var client = new EmailClient(_settings.ConnectionString);
            await client.SendAsync(WaitUntil.Started, message);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Email send operation failed with error code {errorCode}", ex.ErrorCode);
        }
    }
}
