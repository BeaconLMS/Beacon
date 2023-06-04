using Azure.Communication.Email;
using Azure;
using Microsoft.Extensions.Options;
using Beacon.API.App.Services;
using Beacon.API.App.Settings;

namespace Beacon.API.Infrastructure.Email;

internal sealed class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(string subject, string htmlContent, string toAddress)
    {
        //var subject = "Welcome to Azure Communication Service Email APIs.";
        //var htmlContent = "<html><body><h1>Quick send email test</h1><br/><h4>This email message is sent from Azure Communication Service Email.</h4><p>This mail was sent using .NET SDK!!</p></body></html>";

        try
        {
            var emailClient = new EmailClient(_settings.ConnectionString);

            await emailClient.SendAsync(
                WaitUntil.Started,
                _settings.MailFrom,
                toAddress,
                subject,
                htmlContent);
        }
        catch (RequestFailedException ex)
        {
            /// OperationID is contained in the exception message and can be used for troubleshooting purposes
            Console.WriteLine($"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");
        }
    }
}
