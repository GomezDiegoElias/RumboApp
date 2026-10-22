using Microsoft.EntityFrameworkCore;

namespace RumboApp.Trips.Data;

public class TripsDbContext(DbContextOptions<TripsDbContext> options) : DbContext(options)
{
    // public DbSet<Trip> Trips => Set<Trip>();
}