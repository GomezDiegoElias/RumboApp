using NpgsqlTypes;

namespace RumboApp.Trips.Infrastructure.Data;

internal sealed class DriverCommitmentRow
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public Guid TripId { get; set; }
    public NpgsqlRange<DateTimeOffset> Period { get; set; }
}
