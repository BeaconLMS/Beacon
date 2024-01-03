using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityAPI.Database;

public sealed class IdentityDatabaseContext(DbContextOptions<IdentityDatabaseContext> options) 
    : IdentityDbContext(options)
{
}
