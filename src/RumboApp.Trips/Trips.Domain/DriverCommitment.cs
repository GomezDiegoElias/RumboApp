namespace RumboApp.Trips.Domain;

public sealed record DriverCommitment
{
    public DriverCommitment(
        Guid driverId,
        Guid tripId,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt)
    {
        if (driverId == Guid.Empty)
        {
            throw new DomainException("El chofer es obligatorio.");
        }

        if (tripId == Guid.Empty)
        {
            throw new DomainException("El viaje es obligatorio.");
        }

        if (startsAt.Offset != TimeSpan.Zero || endsAt.Offset != TimeSpan.Zero)
        {
            throw new DomainException("El compromiso debe usar fechas UTC.");
        }

        if (endsAt <= startsAt)
        {
            throw new DomainException("El fin del compromiso debe ser posterior al inicio.");
        }

        DriverId = driverId;
        TripId = tripId;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public Guid DriverId { get; }
    public Guid TripId { get; }
    public DateTimeOffset StartsAt { get; }
    public DateTimeOffset EndsAt { get; }
}
