using RumboApp.Trips.Domain;
using Xunit;

namespace RumboApp.Trips.Domain.Tests;

public sealed class TripTests
{
    private static readonly Guid PassengerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid DriverId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly GeoPoint Origin = new(-27.4516, -58.9867);
    private static readonly GeoPoint Destination = new(-27.4600, -58.9800);

    [Fact]
    public void Request_starts_in_requested_state()
    {
        var trip = Trip.Request(PassengerId, Origin, Destination, new Money(1500));

        Assert.Equal(TripState.Requested, trip.State);
        Assert.Null(trip.DriverId);
        Assert.Null(trip.FinalFare);
    }

    [Fact]
    public void Scheduled_trip_requires_a_future_time()
    {
        var now = new DateTimeOffset(2026, 10, 22, 17, 0, 0, TimeSpan.Zero);

        var trip = Trip.Schedule(
            PassengerId,
            Origin,
            Destination,
            now.AddHours(2),
            new Money(1800),
            now);

        Assert.Equal(TripState.Scheduled, trip.State);
        Assert.Equal(now.AddHours(2), trip.ScheduledAt);
    }

    [Fact]
    public void Scheduled_trip_rejects_a_past_time()
    {
        var now = new DateTimeOffset(2026, 10, 22, 17, 0, 0, TimeSpan.Zero);

        var exception = Assert.Throws<DomainException>(() =>
            Trip.Schedule(PassengerId, Origin, Destination, now, new Money(1800), now));

        Assert.Equal("La fecha programada debe estar en el futuro.", exception.Message);
    }

    [Fact]
    public void Assignment_and_completion_preserve_the_business_flow()
    {
        var trip = Trip.Request(PassengerId, Origin, Destination, new Money(1500));

        trip.AssignDriver(DriverId);
        trip.TransitionTo(TripState.DriverEnRoute);
        trip.TransitionTo(TripState.InProgress);
        trip.Complete(new Money(1750));

        Assert.Equal(TripState.Completed, trip.State);
        Assert.Equal(DriverId, trip.DriverId);
        Assert.Equal(new Money(1750), trip.FinalFare);
    }

    [Fact]
    public void Completed_trip_cannot_be_reopened()
    {
        var trip = Trip.Request(PassengerId, Origin, Destination, new Money(1500));
        trip.AssignDriver(DriverId);
        trip.TransitionTo(TripState.DriverEnRoute);
        trip.TransitionTo(TripState.InProgress);
        trip.Complete(new Money(1750));

        var exception = Assert.Throws<DomainException>(() => trip.TransitionTo(TripState.Assigned));

        Assert.Contains("No se permite pasar de Completed a Assigned", exception.Message);
    }

    [Fact]
    public void Invalid_coordinates_are_rejected()
    {
        var exception = Assert.Throws<DomainException>(() => new GeoPoint(91, 0));

        Assert.Equal("La latitud debe estar entre -90 y 90.", exception.Message);
    }

    [Fact]
    public void Non_finite_coordinates_are_rejected()
    {
        var exception = Assert.Throws<DomainException>(() => new GeoPoint(double.NaN, 0));

        Assert.Equal("La latitud debe estar entre -90 y 90.", exception.Message);
    }

    [Fact]
    public void Currency_code_must_be_uppercase()
    {
        var exception = Assert.Throws<DomainException>(() => new Money(100, "ars"));

        Assert.Equal("La moneda debe ser un código de tres letras.", exception.Message);
    }

    [Fact]
    public void Fare_calculation_uses_the_same_policy_for_estimation_and_final_amount()
    {
        var policy = new FarePolicy(
            new Money(500),
            new Money(100),
            new Money(50));

        var estimated = FareCalculator.Calculate(policy, 1_001, 61);
        var final = FareCalculator.Calculate(policy, 1_001, 61);

        Assert.Equal(new Money(800), estimated);
        Assert.Equal(estimated, final);
    }

    [Fact]
    public void Fare_calculation_rounds_partial_distance_and_time_up()
    {
        var policy = new FarePolicy(
            new Money(500),
            new Money(100),
            new Money(50));

        var fare = FareCalculator.Calculate(policy, 1, 1);

        Assert.Equal(new Money(650), fare);
    }

    [Fact]
    public void Fare_calculation_rejects_negative_inputs()
    {
        var exception = Assert.Throws<DomainException>(() =>
            FareCalculator.Calculate(new FarePolicy(new Money(500), new Money(100), new Money(50)), -1, 0));

        Assert.Equal("La distancia no puede ser negativa.", exception.Message);
    }
}
