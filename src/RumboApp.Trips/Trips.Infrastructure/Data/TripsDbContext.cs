using Microsoft.EntityFrameworkCore;

namespace RumboApp.Trips.Infrastructure.Data;

public sealed class TripsDbContext(DbContextOptions<TripsDbContext> options) : DbContext(options)
{
    internal DbSet<TripRow> Trips => Set<TripRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TripRow>(entity =>
        {
            entity.ToTable("trips");
            entity.HasKey(trip => trip.Id);
            entity.Property(trip => trip.State).HasConversion<string>().HasMaxLength(32);
            entity.Property(trip => trip.EstimatedFareCurrency).HasMaxLength(3).IsFixedLength();
            entity.Property(trip => trip.FinalFareCurrency).HasMaxLength(3).IsFixedLength();
            entity.HasIndex(trip => new { trip.PassengerId, trip.State });
        });
    }
}
