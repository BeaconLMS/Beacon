﻿using Beacon.Common.Memberships;
using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;

namespace Beacon.Common.Projects.Requests;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public class CreateProjectRequest : LaboratoryRequestBase, IRequest
{
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;

    public class Validator : AbstractValidator<CreateProjectRequest>
    {
        public Validator()
        {
            RuleFor(x => x.CustomerCode).IsValidCustomerCode();
            RuleFor(x => x.CustomerName).IsValidCustomerName();
        }
    }
}
