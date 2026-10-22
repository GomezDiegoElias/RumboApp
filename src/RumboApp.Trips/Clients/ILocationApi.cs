namespace RumboApp.Trips.Clients;

public interface ILocationApi
{
    Task<bool> PingAsync(CancellationToken ct = default);
}