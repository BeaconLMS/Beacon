namespace Beacon.API.App.Services;

public interface IEmailService
{
    Task SendAsync(string subject, string htmlContent, string toAddress);
}
