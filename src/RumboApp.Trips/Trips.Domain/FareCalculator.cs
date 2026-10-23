namespace RumboApp.Trips.Domain;

public static class FareCalculator
{
    public static Money Calculate(
        FarePolicy policy,
        long distanceMeters,
        long durationSeconds)
    {
        if (distanceMeters < 0)
        {
            throw new DomainException("La distancia no puede ser negativa.");
        }

        if (durationSeconds < 0)
        {
            throw new DomainException("La duración no puede ser negativa.");
        }

        var kilometers = CeilingDivision(distanceMeters, 1_000);
        var minutes = CeilingDivision(durationSeconds, 60);
        var cents = checked(
            policy.BaseFare.Cents
            + (kilometers * policy.PerKilometer.Cents)
            + (minutes * policy.PerMinute.Cents));

        return new Money(cents, policy.BaseFare.Currency);
    }

    private static long CeilingDivision(long value, long divisor) =>
        value == 0 ? 0 : checked((value - 1) / divisor + 1);
}
