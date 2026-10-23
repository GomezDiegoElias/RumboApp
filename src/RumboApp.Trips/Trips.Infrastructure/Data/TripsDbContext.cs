using Microsoft.EntityFrameworkCore;

namespace RumboApp.Trips.Infrastructure.Data;

public sealed class TripsDbContext(DbContextOptions<TripsDbContext> options) : DbContext(options)
{
    internal DbSet<TripRow> Trips => Set<TripRow>();
    internal DbSet<DriverCommitmentRow> DriverCommitments => Set<DriverCommitmentRow>();

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

        modelBuilder.Entity<DriverCommitmentRow>(entity =>
        {
            entity.ToTable("driver_commitments");
            entity.HasKey(commitment => commitment.Id);
            entity.Property(commitment => commitment.Period)
                .HasColumnType("tstzrange");
            entity.HasIndex(commitment => commitment.DriverId);
            entity.HasIndex(commitment => commitment.TripId);
        });
    }
}
