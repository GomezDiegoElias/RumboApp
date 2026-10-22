namespace RumboApp.Web.Clients;

public interface ILocationApi
{
    Task<bool> PingAsync(CancellationToken ct = default);
}