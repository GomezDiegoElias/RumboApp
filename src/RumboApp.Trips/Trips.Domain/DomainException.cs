namespace RumboApp.Trips.Domain;

public sealed class DomainException(string message) : Exception(message);
