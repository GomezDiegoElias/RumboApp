using RumboApp.Trips.Domain;

namespace RumboApp.Trips.Infrastructure.Data;

internal sealed class TripRow
{
    public Guid Id { get; set; }
    public Guid PassengerId { get; set; }
    public double OriginLatitude { get; set; }
    public double OriginLongitude { get; set; }
    public double DestinationLatitude { get; set; }
    public double DestinationLongitude { get; set; }
    public TripState State { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }
    public long EstimatedFareCents { get; set; }
    public string EstimatedFareCurrency { get; set; } = string.Empty;
    public long? FinalFareCents { get; set; }
    public string? FinalFareCurrency { get; set; }
    public Guid? DriverId { get; set; }
}
