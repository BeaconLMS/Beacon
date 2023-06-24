﻿using Beacon.API.Persistence;
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

        public Validator(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(r => r.NewMemberEmailAddress)
                .MustAsync(NotBeAnExistingMember)
                .WithMessage("User is already a member of the specified laboratory.");
        }

        private async Task<bool> NotBeAnExistingMember(InviteLabMemberRequest request, string email, CancellationToken ct)
        {
            var isAMember = await _dbContext.Memberships
                .AnyAsync(m => m.LaboratoryId == request.LaboratoryId && m.Member.EmailAddress == email, ct);

            return !isAMember;
        }
    }

    internal sealed class CommandHandler : IRequestHandler<InviteLabMemberRequest>
    {
        private readonly ApplicationSettings _appSettings;
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;
        private readonly IEmailService _emailService;

        public CommandHandler(IOptions<ApplicationSettings> appSettings, ICurrentUser currentUser, BeaconDbContext dbContext, IEmailService emailService)
        {
            _appSettings = appSettings.Value;
            _currentUser = currentUser;
            _dbContext = dbContext;
            _emailService = emailService;
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
            var membership = await _dbContext.Memberships
                .AsNoTracking()
                .Where(m => m.MemberId == currentUser.Id && m.LaboratoryId == request.LaboratoryId)
                .SingleOrDefaultAsync(ct);

            if (membership?.MembershipType is LaboratoryMembershipType.Admin)
                return;

            if (request.MembershipType is LaboratoryMembershipType.Admin)
                throw new UserNotAllowedException("Only laboratory admins are allowed to invite new admins.");

            if (membership?.MembershipType is not LaboratoryMembershipType.Manager)
                throw new UserNotAllowedException("Only laboratory admins or managers are allowed to invite new members.");
        }

        private async Task<InvitationEmail> CreateInvitation(InviteLabMemberRequest request, User currentUser, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                CreatedOn = now,
                ExpireAfterDays = 10, // TODO: make this configurable
                NewMemberEmailAddress = request.NewMemberEmailAddress,
                MembershipType = request.MembershipType,
                Laboratory = await _dbContext.Laboratories.SingleAsync(x => x.Id == request.LaboratoryId, ct),
                LaboratoryId = request.LaboratoryId,
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