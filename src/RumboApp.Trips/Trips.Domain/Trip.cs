namespace RumboApp.Trips.Domain;

public sealed class Trip
{
    private Trip(
        Guid id,
        Guid passengerId,
        GeoPoint origin,
        GeoPoint destination,
        TripState state,
        DateTimeOffset? scheduledAt,
        Money estimatedFare)
    {
        if (passengerId == Guid.Empty)
        {
            throw new DomainException("El pasajero es obligatorio.");
        }

        if (origin == destination)
        {
            throw new DomainException("El origen y el destino deben ser distintos.");
        }

        if (state == TripState.Scheduled && scheduledAt is null)
        {
            throw new DomainException("Un viaje programado requiere fecha y hora.");
        }

        Id = id;
        PassengerId = passengerId;
        Origin = origin;
        Destination = destination;
        State = state;
        ScheduledAt = scheduledAt;
        EstimatedFare = estimatedFare;
    }

    public Guid Id { get; }
    public Guid PassengerId { get; }
    public GeoPoint Origin { get; }
    public GeoPoint Destination { get; }
    public TripState State { get; private set; }
    public DateTimeOffset? ScheduledAt { get; }
    public Money EstimatedFare { get; }
    public Money? FinalFare { get; private set; }
    public Guid? DriverId { get; private set; }

    public static Trip Request(
        Guid passengerId,
        GeoPoint origin,
        GeoPoint destination,
        Money estimatedFare,
        Guid? id = null) =>
        new(id ?? Guid.NewGuid(), passengerId, origin, destination, TripState.Requested, null, estimatedFare);

    public static Trip Schedule(
        Guid passengerId,
        GeoPoint origin,
        GeoPoint destination,
        DateTimeOffset scheduledAt,
        Money estimatedFare,
        DateTimeOffset now,
        Guid? id = null)
    {
        if (scheduledAt <= now)
        {
            throw new DomainException("La fecha programada debe estar en el futuro.");
        }

        return new(id ?? Guid.NewGuid(), passengerId, origin, destination, TripState.Scheduled, scheduledAt, estimatedFare);
    }

    public static Trip Rehydrate(
        Guid id,
        Guid passengerId,
        GeoPoint origin,
        GeoPoint destination,
        TripState state,
        DateTimeOffset? scheduledAt,
        Money estimatedFare,
        Guid? driverId,
        Money? finalFare)
    {
        var trip = new Trip(id, passengerId, origin, destination, state, scheduledAt, estimatedFare)
        {
            DriverId = driverId,
            FinalFare = finalFare
        };

        return trip;
    }

    public void AssignDriver(Guid driverId)
    {
        if (driverId == Guid.Empty)
        {
            throw new DomainException("El chofer es obligatorio.");
        }

        TransitionTo(TripState.Assigned);
        DriverId = driverId;
    }

    public void Complete(Money finalFare)
    {
        TransitionTo(TripState.Completed);
        FinalFare = finalFare;
    }

    public void TransitionTo(TripState target)
    {
        if (!TripStateMachine.CanTransition(State, target))
        {
            throw new DomainException($"No se permite pasar de {State} a {target}.");
        }

        State = target;
    }
}
