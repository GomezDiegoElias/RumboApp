namespace RumboApp.Trips.Domain.Ports;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
