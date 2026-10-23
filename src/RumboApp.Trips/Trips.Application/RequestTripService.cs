using RumboApp.Trips.Domain;
using RumboApp.Trips.Domain.Ports;

namespace RumboApp.Trips.Application;

public sealed class RequestTripService(
    ITripRepository tripRepository,
    IUnitOfWork unitOfWork,
    FarePolicy farePolicy)
{
    public async Task<Trip> ExecuteAsync(
        RequestTripCommand command,
        CancellationToken cancellationToken = default)
    {
        var estimatedFare = FareCalculator.Calculate(
            farePolicy,
            command.EstimatedDistanceMeters,
            command.EstimatedDurationSeconds);

        var trip = Trip.Request(
            command.PassengerId,
            command.Origin,
            command.Destination,
            estimatedFare);

        await tripRepository.AddAsync(trip, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return trip;
    }
}
