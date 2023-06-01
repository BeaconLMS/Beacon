using MediatR;

namespace Beacon.Common.Auth.Events;

public sealed record LoginEvent(AuthUserDto LoggedInUser) : INotification;
