namespace RumboApp.Web.Clients;

public interface IIdentityApi
{
    Task<bool> PingAsync(CancellationToken ct = default);
}