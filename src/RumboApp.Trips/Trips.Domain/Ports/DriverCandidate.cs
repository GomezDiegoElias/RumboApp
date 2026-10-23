namespace RumboApp.Trips.Domain.Ports;

public readonly record struct DriverCandidate(
    Guid DriverId,
    GeoPoint Location,
    long DistanceMeters);
