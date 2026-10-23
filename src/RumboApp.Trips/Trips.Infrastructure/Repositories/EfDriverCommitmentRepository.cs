using NpgsqlTypes;
using RumboApp.Trips.Domain;
using RumboApp.Trips.Domain.Ports;
using RumboApp.Trips.Infrastructure.Data;

namespace RumboApp.Trips.Infrastructure.Repositories;

public sealed class EfDriverCommitmentRepository(TripsDbContext dbContext)
    : IDriverCommitmentRepository
{
    public Task AddAsync(
        DriverCommitment commitment,
        CancellationToken cancellationToken = default)
    {
        dbContext.DriverCommitments.Add(new DriverCommitmentRow
        {
            Id = Guid.NewGuid(),
            DriverId = commitment.DriverId,
            TripId = commitment.TripId,
            Period = new NpgsqlRange<DateTimeOffset>(
                commitment.StartsAt,
                commitment.EndsAt)
        });

        return Task.CompletedTask;
    }
}
