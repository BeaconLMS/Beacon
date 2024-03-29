﻿namespace Beacon.Common.Models;

public sealed record LaboratoryDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MemberCount { get; init; }
}
