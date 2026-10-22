using Microsoft.EntityFrameworkCore;

namespace RumboApp.Location.Data;

public class LocationDbContext(DbContextOptions<LocationDbContext> options) : DbContext(options)
{
    // public DbSet<Place> Places => Set<Place>();
}