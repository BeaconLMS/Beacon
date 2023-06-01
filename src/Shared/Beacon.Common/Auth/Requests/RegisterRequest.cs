﻿using FluentValidation;

namespace Beacon.Common.Auth.Requests;

public class RegisterRequest : IApiRequest<AuthUserDto>
{
    public string DisplayName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public class Validator : AbstractValidator<RegisterRequest>
    {
        public Validator()
        {
            RuleFor(r => r.EmailAddress)
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(r => r.DisplayName)
                .NotEmpty().WithMessage("Display name is required.");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }

}
