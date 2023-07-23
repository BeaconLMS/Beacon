﻿using Beacon.Common.Models;

namespace Beacon.Common.Requests.Laboratories;

public sealed class GetLaboratoryEventsRequest : BeaconRequest<GetLaboratoryEventsRequest, LaboratoryEventDto[]>
{
    public DateOnly? MinDate { get; set; }
    public DateOnly? MaxDate { get; set; }
}
