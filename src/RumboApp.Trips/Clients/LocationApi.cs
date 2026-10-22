namespace RumboApp.Trips.Clients;

public class LocationApi(HttpClient http) : ILocationApi
{
    public async Task<bool> PingAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync("/health", ct);
        return response.IsSuccessStatusCode;
    }
}