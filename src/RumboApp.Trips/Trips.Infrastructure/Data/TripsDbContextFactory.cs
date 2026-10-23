using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RumboApp.Trips.Infrastructure.Data;

public sealed class TripsDbContextFactory : IDesignTimeDbContextFactory<TripsDbContext>
{
    public TripsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("TRIPS_DB_CONNECTION")
            ?? throw new InvalidOperationException(
                "La variable TRIPS_DB_CONNECTION es obligatoria para generar migraciones.");

        var options = new DbContextOptionsBuilder<TripsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TripsDbContext(options);
    }
}
