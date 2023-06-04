using Beacon.API.App.Settings;
using Beacon.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Beacon.API.App.Services.Email;

public sealed class LabInvitationEmailService
{
    private readonly ApplicationSettings _appSettings;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public LabInvitationEmailService(IOptions<ApplicationSettings> appSettings, IEmailService emailService, IUnitOfWork unitOfWork)
    {
        _appSettings = appSettings.Value;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task SendAsync(Guid emailInvitationId, CancellationToken ct)
    {
        var emailInvitation = await _unitOfWork
            .QueryFor<LaboratoryInvitationEmail>(enableChangeTracking: true)
            .Include(l => l.LaboratoryInvitation)
                .ThenInclude(i => i.CreatedBy)
            .Include(l => l.LaboratoryInvitation)
                .ThenInclude(i => i.Laboratory)
            .Include(l => l.DeliveryStatus)
            .FirstOrDefaultAsync(l => l.Id == emailInvitationId, ct)
            ?? throw new Exception("Email invitation not found."); // TODO: throw better exception

        var emailSendOperation = await _emailService.SendAsync(
            GetSubject(emailInvitation.LaboratoryInvitation),
            GetBody(_appSettings.BaseUrl, emailInvitation),
            emailInvitation.LaboratoryInvitation.NewMemberEmailAddress)
            ?? throw new Exception("There was an error sending the invitation email."); // TODO: throw better exception

        emailInvitation.MarkAsSent(emailSendOperation.OperationId, emailSendOperation.Timestamp);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static string GetSubject(LaboratoryInvitation invitation)
    {
        return $"{invitation.CreatedBy.DisplayName} invites you to join a lab!";

    }
    private static string GetAcceptUrl(string baseUrl, LaboratoryInvitationEmail invitation)
    {
        var labId = invitation.LaboratoryInvitation.LaboratoryId;
        return $"{baseUrl}/laboratories/{labId}/acceptInvite?key={invitation.Id}";
    }

    private static string GetBody(string baseUrl, LaboratoryInvitationEmail emailInvitation)
    {
        var invitation = emailInvitation.LaboratoryInvitation;
        var labName = invitation.Laboratory.Name;
        var expirationDays = invitation.ExpirationTimeSpan.Days;

        return $"""
            <p>
                <h3>You're invited to join {labName}!</h3>
                <p>
                    <a href="{GetAcceptUrl(baseUrl, emailInvitation)}">Click here to accept.</a>
                    <br />
                    <small>This invitation will expire in {expirationDays} days.</small>
                </p>
            </p>
            """;
    }
}
