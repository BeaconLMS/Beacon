﻿using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.App.Settings;
using Beacon.Common.Invitations;
using Beacon.Common.Memberships;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Beacon.API.Endpoints.Invitations;

public sealed class CreateInvitation : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost<InviteLabMemberRequest>("invitations").WithTags(EndpointTags.Invitations);
    }

    public sealed class Validator : AbstractValidator<InviteLabMemberRequest>
    {
        private readonly BeaconDbContext _dbContext;
        private readonly LaboratoryContext _labContext;

        public Validator(BeaconDbContext dbContext, LaboratoryContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;

            RuleFor(r => r.NewMemberEmailAddress)
                .MustAsync(NotBeAnExistingMember)
                .WithMessage("User is already a member of the specified laboratory.");
        }

        private async Task<bool> NotBeAnExistingMember(InviteLabMemberRequest request, string email, CancellationToken ct)
        {
            var labId = _labContext.LaboratoryId;

            var isAMember = await _dbContext.Memberships
                .AnyAsync(m => m.LaboratoryId == labId && m.Member.EmailAddress == email, ct);

            return !isAMember;
        }
    }

    internal sealed class CommandHandler : IRequestHandler<InviteLabMemberRequest>
    {
        private readonly ApplicationSettings _appSettings;
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly LaboratoryContext _labContext;

        public CommandHandler(IOptions<ApplicationSettings> appSettings, ICurrentUser currentUser, BeaconDbContext dbContext, IEmailService emailService, LaboratoryContext labContext)
        {
            _appSettings = appSettings.Value;
            _currentUser = currentUser;
            _dbContext = dbContext;
            _emailService = emailService;
            _labContext = labContext;
        }

        public async Task Handle(InviteLabMemberRequest request, CancellationToken ct)
        {
            var user = await GetCurrentUser(ct);
            await EnsureCurrentUserIsAllowed(request, user, ct);

            var invitation = await CreateInvitation(request, user, ct);
            await SendAsync(invitation, ct);
        }

        private async Task<User> GetCurrentUser(CancellationToken ct)
        {
            var id = _currentUser.UserId;
            return await _dbContext.Users.SingleAsync(x => x.Id == id, ct);
        }

        private async Task EnsureCurrentUserIsAllowed(InviteLabMemberRequest request, User currentUser, CancellationToken ct)
        {
            var labId = _labContext.LaboratoryId;

            var membership = await _dbContext.Memberships
                .Where(m => m.MemberId == currentUser.Id && m.LaboratoryId == labId)
                .AsNoTracking()
                .SingleAsync(ct);

            if (membership.MembershipType is LaboratoryMembershipType.Manager && request.MembershipType is LaboratoryMembershipType.Admin)
                throw new UserNotAllowedException("Only laboratory admins are allowed to invite new admins.");
        }

        private async Task<InvitationEmail> CreateInvitation(InviteLabMemberRequest request, User currentUser, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;
            var labId = _labContext.LaboratoryId;

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                CreatedOn = now,
                ExpireAfterDays = 10, // TODO: make this configurable
                NewMemberEmailAddress = request.NewMemberEmailAddress,
                MembershipType = request.MembershipType,
                Laboratory = await _dbContext.Laboratories.SingleAsync(x => x.Id == labId, ct),
                LaboratoryId = labId,
                CreatedById = currentUser.Id,
                CreatedBy = currentUser
            };

            var emailInvitation = invitation.AddEmailInvitation(now);

            _dbContext.Invitations.Add(invitation);
            await _dbContext.SaveChangesAsync(ct);

            return emailInvitation;
        }

        private async Task SendAsync(InvitationEmail emailInvitation, CancellationToken ct)
        {
            var emailSendOperation = await _emailService.SendAsync(
                GetSubject(emailInvitation.LaboratoryInvitation),
                GetBody(_appSettings.BaseUrl, emailInvitation),
                emailInvitation.LaboratoryInvitation.NewMemberEmailAddress)
                ?? throw new Exception("There was an error sending the invitation email."); // TODO: throw better exception

            emailInvitation.OperationId = emailSendOperation.OperationId;

            await _dbContext.SaveChangesAsync(ct);
        }

        private static string GetSubject(Invitation invitation)
        {
            return $"{invitation.CreatedBy.DisplayName} invites you to join a lab!";

        }

        private static string GetAcceptUrl(string baseUrl, InvitationEmail invitation)
        {
            return $"{baseUrl}/invitations/{invitation.LaboratoryInvitationId}/accept?emailId={invitation.Id}";
        }

        private static string GetBody(string baseUrl, InvitationEmail emailInvitation)
        {
            var invitation = emailInvitation.LaboratoryInvitation;

            return $"""
            <p>
                <h3>You're invited to join {invitation.Laboratory.Name}!</h3>
                <p>
                    <a href="{GetAcceptUrl(baseUrl, emailInvitation)}">Click here to accept.</a>
                    <br />
                    <small>This invitation will expire in {invitation.ExpireAfterDays} days.</small>
                </p>
            </p>
            """;
        }
    }
}
