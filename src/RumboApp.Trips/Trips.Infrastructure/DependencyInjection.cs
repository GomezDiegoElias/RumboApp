using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using RumboApp.Trips.Domain.Ports;
using RumboApp.Trips.Domain;
using RumboApp.Trips.Infrastructure.Configuration;
using RumboApp.Trips.Infrastructure.Data;
using RumboApp.Trips.Infrastructure.Repositories;

namespace RumboApp.Trips.Infrastructure;

public static class DependencyInjection
{
    public static async Task ApplyTripsMigrationsAsync(
        this IHost app,
        CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TripsDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public static IHostApplicationBuilder AddTripsPersistence(
        this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<TripsDbContext>("tripsdb");
        builder.Services
            .AddOptions<FarePolicyOptions>()
            .BindConfiguration("Trips:Fare")
            .ValidateOnStart();
        builder.Services.AddSingleton<IValidateOptions<FarePolicyOptions>, FarePolicyOptionsValidator>();
        builder.Services.AddSingleton(typeof(FarePolicy), serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<FarePolicyOptions>>().Value.ToDomain());
        builder.Services.AddScoped<ITripRepository, EfTripRepository>();
        builder.Services.AddScoped<IDriverCommitmentRepository, EfDriverCommitmentRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        return builder;
    }
}
