using Microsoft.Extensions.Options;
using RumboApp.Trips.Domain;
using RumboApp.Trips.Infrastructure.Configuration;
using Xunit;

namespace RumboApp.Trips.Infrastructure.Tests;

public sealed class FarePolicyOptionsValidatorTests
{
    [Fact]
    public void Valid_options_are_accepted_and_mapped_to_domain()
    {
        var options = new FarePolicyOptions
        {
            BaseFareCents = 500,
            PerKilometerCents = 100,
            PerMinuteCents = 50,
            Currency = "ARS"
        };

        var result = new FarePolicyOptionsValidator().Validate(null, options);

        Assert.True(result.Succeeded);
        Assert.Equal(new Money(500), options.ToDomain().BaseFare);
    }

    [Fact]
    public void Missing_currency_is_rejected()
    {
        var options = new FarePolicyOptions
        {
            BaseFareCents = 500,
            PerKilometerCents = 100,
            PerMinuteCents = 50
        };

        var result = new FarePolicyOptionsValidator().Validate(null, options);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Failures ?? [], failure => failure.Contains("Currency"));
    }

    [Fact]
    public void Lowercase_currency_is_rejected()
    {
        var options = new FarePolicyOptions
        {
            BaseFareCents = 500,
            PerKilometerCents = 100,
            PerMinuteCents = 50,
            Currency = "ars"
        };

        var result = new FarePolicyOptionsValidator().Validate(null, options);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Failures ?? [], failure => failure.Contains("Currency"));
    }

    [Fact]
    public void Negative_values_are_rejected()
    {
        var options = new FarePolicyOptions
        {
            BaseFareCents = -1,
            PerKilometerCents = 100,
            PerMinuteCents = 50,
            Currency = "ARS"
        };

        var result = new FarePolicyOptionsValidator().Validate(null, options);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Failures ?? [], failure => failure.Contains("BaseFareCents"));
    }
}
