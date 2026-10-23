using RumboApp.Trips.Domain;
using Xunit;

namespace RumboApp.Trips.Domain.Tests;

public sealed class DriverCommitmentTests
{
    private static readonly Guid DriverId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid TripId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly DateTimeOffset StartsAt =
        new(2026, 10, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Commitment_accepts_a_valid_utc_interval()
    {
        var commitment = new DriverCommitment(
            DriverId,
            TripId,
            StartsAt,
            StartsAt.AddHours(1));

        Assert.Equal(DriverId, commitment.DriverId);
        Assert.Equal(TripId, commitment.TripId);
        Assert.Equal(StartsAt.AddHours(1), commitment.EndsAt);
    }

    [Fact]
    public void Commitment_rejects_non_utc_dates()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new DriverCommitment(
                DriverId,
                TripId,
                StartsAt.ToOffset(TimeSpan.FromHours(-3)),
                StartsAt.AddHours(1)));

        Assert.Equal("El compromiso debe usar fechas UTC.", exception.Message);
    }

    [Fact]
    public void Commitment_rejects_an_empty_interval()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new DriverCommitment(DriverId, TripId, StartsAt, StartsAt));

        Assert.Equal(
            "El fin del compromiso debe ser posterior al inicio.",
            exception.Message);
    }
}
