﻿using FluentValidation;

namespace Beacon.Common.Requests.Projects.Events;

public sealed class AssociateInstrumentWithProjectEventRequest : BeaconRequest<AssociateInstrumentWithProjectEventRequest>
{
    public Guid ProjectEventId { get; set; }
    public Guid InstrumentId { get; set; }

    public sealed class Validator : AbstractValidator<UnassociateInstrumentFromProjectEventRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectEventId).NotEmpty().WithMessage("Project event must be specified.");
            RuleFor(x => x.InstrumentId).NotEmpty().WithMessage("Instrument must be specified.");
        }
    }
}
