using MediatR;

namespace Beacon.Common.Laboratories;

public class CreateNewLaboratoryRequest : IRequest<LaboratorySummaryDto>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
