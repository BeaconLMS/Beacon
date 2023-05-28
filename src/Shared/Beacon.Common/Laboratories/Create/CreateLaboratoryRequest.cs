using MediatR;

namespace Beacon.Common.Laboratories.Create;

public class CreateLaboratoryRequest : IBeaconRequest<LaboratorySummaryDto>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
