namespace RumboApp.Web.Clients;

public interface ITripsApi
{
    Task<bool> PingAsync(CancellationToken ct = default);
}