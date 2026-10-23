using Microsoft.EntityFrameworkCore;
using RumboApp.Trips.Domain;
using RumboApp.Trips.Infrastructure.Data;
using RumboApp.Trips.Infrastructure.Repositories;
using Testcontainers.PostgreSql;
using Xunit;

namespace RumboApp.Trips.Infrastructure.Tests;

public sealed class EfTripRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("tripsdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private TripsDbContext _dbContext = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<TripsDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _dbContext = new TripsDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Repository_persists_and_rehydrates_a_trip()
    {
        var repository = new EfTripRepository(_dbContext);
        var trip = Trip.Schedule(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            new GeoPoint(-27.4516, -58.9867),
            new GeoPoint(-27.4600, -58.9800),
            new DateTimeOffset(2026, 10, 23, 12, 0, 0, TimeSpan.Zero),
            new Money(1800),
            new DateTimeOffset(2026, 10, 22, 23, 0, 0, TimeSpan.Zero),
            Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"));

        await repository.AddAsync(trip);
        var stored = await repository.GetByIdAsync(trip.Id);

        Assert.NotNull(stored);
        Assert.Equal(trip.Id, stored.Id);
        Assert.Equal(trip.PassengerId, stored.PassengerId);
        Assert.Equal(trip.Origin, stored.Origin);
        Assert.Equal(trip.Destination, stored.Destination);
        Assert.Equal(TripState.Scheduled, stored.State);
        Assert.Equal(trip.ScheduledAt, stored.ScheduledAt);
        Assert.Equal(trip.EstimatedFare, stored.EstimatedFare);
    }
}
