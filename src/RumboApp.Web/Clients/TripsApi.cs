namespace RumboApp.Web.Clients;

public class TripsApi(HttpClient http) : ITripsApi
{
    public async Task<bool> PingAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync("/health", ct);
        return response.IsSuccessStatusCode;
    }
}