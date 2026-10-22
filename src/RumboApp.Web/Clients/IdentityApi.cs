namespace RumboApp.Web.Clients;

public class IdentityApi(HttpClient http) : IIdentityApi
{
    public async Task<bool> PingAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync("/health", ct);
        return response.IsSuccessStatusCode;
    }
}