using RumboApp.Trips.Clients;
using RumboApp.Trips.Application;
using RumboApp.Trips.Domain;
using RumboApp.Trips.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddTripsPersistence();
builder.Services.AddScoped<RequestTripService>();

builder.Services.AddHttpClient<ILocationApi, LocationApi>(client =>
{
    client.BaseAddress = new Uri("https+http://location");
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.MapPost("/trips", async (
    RequestTripRequest request,
    RequestTripService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        var trip = await service.ExecuteAsync(
            new RequestTripCommand(
                request.PassengerId,
                new GeoPoint(request.OriginLatitude, request.OriginLongitude),
                new GeoPoint(request.DestinationLatitude, request.DestinationLongitude),
                request.EstimatedDistanceMeters,
                request.EstimatedDurationSeconds),
            cancellationToken);

        return Results.Created($"/trips/{trip.Id}", new
        {
            trip.Id,
            trip.State,
            trip.EstimatedFare,
            trip.ScheduledAt
        });
    }
    catch (DomainException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
});

app.Run();

public sealed record RequestTripRequest(
    Guid PassengerId,
    double OriginLatitude,
    double OriginLongitude,
    double DestinationLatitude,
    double DestinationLongitude,
    long EstimatedDistanceMeters,
    long EstimatedDurationSeconds);