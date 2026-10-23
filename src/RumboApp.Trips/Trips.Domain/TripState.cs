namespace RumboApp.Trips.Domain;

public enum TripState
{
    Requested,
    Scheduled,
    SearchingDriver,
    Assigned,
    DriverEnRoute,
    InProgress,
    Completed,
    CancelledByPassenger,
    CancelledByDriver,
    NoDriversAvailable
}
