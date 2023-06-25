﻿using Beacon.Common.Memberships;
using MediatR;

namespace Beacon.Common.Projects.Requests;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class CancelProjectRequest : IRequest
{
    public required Guid ProjectId { get; set; }
}
