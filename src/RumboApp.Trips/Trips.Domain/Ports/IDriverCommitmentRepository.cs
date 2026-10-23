namespace RumboApp.Trips.Domain.Ports;

public interface IDriverCommitmentRepository
{
    Task AddAsync(
        DriverCommitment commitment,
        CancellationToken cancellationToken = default);
}
