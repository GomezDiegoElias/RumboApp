namespace RumboApp.Trips.Domain.Ports;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
