namespace RumboApp.Trips.Domain.Ports;

public interface ITripRepository
{
    Task<Trip?> GetByIdAsync(Guid tripId, CancellationToken cancellationToken = default);

    Task AddAsync(Trip trip, CancellationToken cancellationToken = default);
}
