namespace RumboApp.Trips.Domain.Ports;

public interface IDriverCandidateProvider
{
    Task<IReadOnlyList<DriverCandidate>> FindNearbyAsync(
        GeoPoint origin,
        CancellationToken cancellationToken = default);
}
