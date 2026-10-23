using RumboApp.Trips.Application;
using RumboApp.Trips.Domain;
using RumboApp.Trips.Domain.Ports;
using Xunit;

namespace RumboApp.Trips.Application.Tests;

public sealed class RequestTripServiceTests
{
    private static readonly Guid PassengerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly GeoPoint Origin = new(-27.4516, -58.9867);
    private static readonly GeoPoint Destination = new(-27.4600, -58.9800);

    [Fact]
    public async Task Execute_creates_a_trip_with_the_estimated_fare()
    {
        var repository = new InMemoryTripRepository();
        var service = new RequestTripService(
            repository,
            new FarePolicy(new Money(500), new Money(100), new Money(50)));

        var trip = await service.ExecuteAsync(new RequestTripCommand(
            PassengerId,
            Origin,
            Destination,
            1_001,
            61));

        Assert.Same(trip, repository.SavedTrip);
        Assert.Equal(TripState.Requested, trip.State);
        Assert.Equal(new Money(800), trip.EstimatedFare);
    }

    private sealed class InMemoryTripRepository : ITripRepository
    {
        public Trip? SavedTrip { get; private set; }

        public Task AddAsync(Trip trip, CancellationToken cancellationToken = default)
        {
            SavedTrip = trip;
            return Task.CompletedTask;
        }

        public Task<Trip?> GetByIdAsync(Guid tripId, CancellationToken cancellationToken = default) =>
            Task.FromResult(SavedTrip?.Id == tripId ? SavedTrip : null);
    }

}
