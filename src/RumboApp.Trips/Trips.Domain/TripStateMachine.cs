namespace RumboApp.Trips.Domain;

public static class TripStateMachine
{
    private static readonly IReadOnlyDictionary<TripState, IReadOnlySet<TripState>> Transitions =
        new Dictionary<TripState, IReadOnlySet<TripState>>
        {
            [TripState.Requested] = Set(
                TripState.Assigned,
                TripState.CancelledByPassenger,
                TripState.NoDriversAvailable),
            [TripState.Scheduled] = Set(
                TripState.SearchingDriver,
                TripState.CancelledByPassenger),
            [TripState.SearchingDriver] = Set(
                TripState.Assigned,
                TripState.NoDriversAvailable,
                TripState.CancelledByPassenger),
            [TripState.Assigned] = Set(
                TripState.DriverEnRoute,
                TripState.CancelledByPassenger,
                TripState.CancelledByDriver),
            [TripState.DriverEnRoute] = Set(
                TripState.InProgress,
                TripState.CancelledByPassenger,
                TripState.CancelledByDriver),
            [TripState.InProgress] = Set(
                TripState.Completed,
                TripState.CancelledByPassenger,
                TripState.CancelledByDriver),
            [TripState.Completed] = Set(),
            [TripState.CancelledByPassenger] = Set(),
            [TripState.CancelledByDriver] = Set(),
            [TripState.NoDriversAvailable] = Set()
        };

    public static bool CanTransition(TripState current, TripState target) =>
        Transitions.TryGetValue(current, out var allowed) && allowed.Contains(target);

    private static IReadOnlySet<TripState> Set(params TripState[] states) =>
        new HashSet<TripState>(states);
}
