using Beacon.API.Persistence;

namespace Beacon.IntegrationTests;

public static class Utilities
{
    public static void EnsureSeeded(BeaconDbContext db)
    {
        db.Database.EnsureCreated();

        if (!db.Users.Any())
        {
            db.Users.Add(CurrentUserDefaults.UserEntity);
            db.SaveChanges();
        }       
    }
}
