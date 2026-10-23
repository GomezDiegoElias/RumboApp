namespace RumboApp.Trips.Domain;

public sealed class DriverNotAvailableException(string message) : DomainException(message);
