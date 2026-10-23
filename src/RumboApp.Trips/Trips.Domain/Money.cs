namespace RumboApp.Trips.Domain;

public readonly record struct Money
{
    public Money(long cents, string currency = "ARS")
    {
        if (cents < 0)
        {
            throw new DomainException("El importe no puede ser negativo.");
        }

        if (string.IsNullOrWhiteSpace(currency)
            || currency.Length != 3
            || currency.Any(character => character is < 'A' or > 'Z'))
        {
            throw new DomainException("La moneda debe ser un código de tres letras.");
        }

        Cents = cents;
        Currency = currency;
    }

    public long Cents { get; }
    public string Currency { get; }
}
