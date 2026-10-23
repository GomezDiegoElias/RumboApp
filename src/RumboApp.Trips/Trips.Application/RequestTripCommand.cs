using RumboApp.Trips.Domain;

namespace RumboApp.Trips.Application;

public sealed record RequestTripCommand(
    Guid PassengerId,
    GeoPoint Origin,
    GeoPoint Destination,
    long EstimatedDistanceMeters,
    long EstimatedDurationSeconds);
