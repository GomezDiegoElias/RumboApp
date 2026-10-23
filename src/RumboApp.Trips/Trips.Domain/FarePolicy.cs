namespace RumboApp.Trips.Domain;

public readonly record struct FarePolicy
{
    public FarePolicy(Money baseFare, Money perKilometer, Money perMinute)
    {
        if (baseFare.Currency != perKilometer.Currency
            || baseFare.Currency != perMinute.Currency)
        {
            throw new DomainException("Todos los importes deben usar la misma moneda.");
        }

        BaseFare = baseFare;
        PerKilometer = perKilometer;
        PerMinute = perMinute;
    }

    public Money BaseFare { get; }
    public Money PerKilometer { get; }
    public Money PerMinute { get; }
}
