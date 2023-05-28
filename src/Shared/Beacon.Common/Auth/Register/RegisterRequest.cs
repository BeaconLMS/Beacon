﻿using MediatR;

namespace Beacon.Common.Auth.Register;

public class RegisterRequest : IRequest<AuthenticatedUserInfo>
{
    public string DisplayName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
