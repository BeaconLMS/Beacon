using Beacon.API.Auth.Services;
using Beacon.API.Entities;
using Beacon.API.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Users;

public static class Register
{
    public sealed record Command : IRequest
    {
        public Guid UserId { get; private init; } = Guid.NewGuid();
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
        public required string PlainTextPassword { get; init; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        private readonly BeaconDbContext _dbContext;

        public CommandValidator(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(r => r.EmailAddress)
                .EmailAddress().WithMessage("Invalid email address.")
                .MustAsync(BeAUniqueEmailAddress).WithMessage("Email address is already in use.");

            RuleFor(r => r.DisplayName)
                .NotEmpty().WithMessage("Display name is required.");

            RuleFor(r => r.PlainTextPassword)
                .NotEmpty().WithMessage("Password is required.");
        }

        private async Task<bool> BeAUniqueEmailAddress(string emailAddress, CancellationToken ct)
        {
            var emailExists = await _dbContext.Users.AnyAsync(u => u.EmailAddress == emailAddress, ct);
            return !emailExists;
        }
    }

    public sealed class CommandHandler : IRequestHandler<Command>
    {
        private readonly BeaconDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public CommandHandler(BeaconDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            _dbContext.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                DisplayName = request.DisplayName,
                EmailAddress = request.EmailAddress,
                HashedPassword = _passwordHasher.Hash(request.PlainTextPassword, out var salt),
                HashedPasswordSalt = salt
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
