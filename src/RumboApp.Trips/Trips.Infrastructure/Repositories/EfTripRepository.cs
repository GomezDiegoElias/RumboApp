using Microsoft.EntityFrameworkCore;
using RumboApp.Trips.Domain;
using RumboApp.Trips.Domain.Ports;
using RumboApp.Trips.Infrastructure.Data;

namespace RumboApp.Trips.Infrastructure.Repositories;

public sealed class EfTripRepository(TripsDbContext dbContext) : ITripRepository
{
    public async Task<Trip?> GetByIdAsync(
        Guid tripId,
        CancellationToken cancellationToken = default)
    {
        var row = await dbContext.Trips
            .AsNoTracking()
            .SingleOrDefaultAsync(trip => trip.Id == tripId, cancellationToken);

        return row is null ? null : ToDomain(row);
    }

    public async Task AddAsync(
        Trip trip,
        CancellationToken cancellationToken = default)
    {
        dbContext.Trips.Add(ToRow(trip));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static TripRow ToRow(Trip trip) =>
        new()
        {
            Id = trip.Id,
            PassengerId = trip.PassengerId,
            OriginLatitude = trip.Origin.Latitude,
            OriginLongitude = trip.Origin.Longitude,
            DestinationLatitude = trip.Destination.Latitude,
            DestinationLongitude = trip.Destination.Longitude,
            State = trip.State,
            ScheduledAt = trip.ScheduledAt,
            EstimatedFareCents = trip.EstimatedFare.Cents,
            EstimatedFareCurrency = trip.EstimatedFare.Currency,
            FinalFareCents = trip.FinalFare?.Cents,
            FinalFareCurrency = trip.FinalFare?.Currency,
            DriverId = trip.DriverId
        };

    private static Trip ToDomain(TripRow row) =>
        Trip.Rehydrate(
            row.Id,
            row.PassengerId,
            new GeoPoint(row.OriginLatitude, row.OriginLongitude),
            new GeoPoint(row.DestinationLatitude, row.DestinationLongitude),
            row.State,
            row.ScheduledAt,
            new Money(row.EstimatedFareCents, row.EstimatedFareCurrency),
            row.DriverId,
            row.FinalFareCents is null
                ? null
                : new Money(row.FinalFareCents.Value, row.FinalFareCurrency!));
}
