namespace Beacon.API.App.Services;

public interface IUnitOfWork
{
    IRepository<T> Get<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken ct);
}
