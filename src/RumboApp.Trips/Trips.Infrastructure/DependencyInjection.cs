using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RumboApp.Trips.Domain.Ports;
using RumboApp.Trips.Infrastructure.Data;
using RumboApp.Trips.Infrastructure.Repositories;

namespace RumboApp.Trips.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddTripsPersistence(
        this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<TripsDbContext>("tripsdb");
        builder.Services.AddScoped<ITripRepository, EfTripRepository>();
        return builder;
    }
}
