﻿using Beacon.Common.Laboratories.Events;
using BeaconUI.Core.Laboratories.Services;
using MediatR;

namespace BeaconUI.Core.Laboratories.EventHandlers;

internal class CreateNewLaboratoryEventHandler : INotificationHandler<LaboratoryCreatedEvent>
{
    private readonly CurrentUserMembershipProvider _membershipProvider;

    public CreateNewLaboratoryEventHandler(CurrentUserMembershipProvider membershipProvider)
    {
        _membershipProvider = membershipProvider;
    }

    public Task Handle(LaboratoryCreatedEvent notification, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _membershipProvider.RefreshState();
        return Task.CompletedTask;
    }
}
