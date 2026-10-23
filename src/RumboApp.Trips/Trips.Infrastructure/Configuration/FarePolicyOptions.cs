using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using RumboApp.Trips.Domain;

namespace RumboApp.Trips.Infrastructure.Configuration;

public sealed class FarePolicyOptions
{
    [Range(0, long.MaxValue)]
    public long BaseFareCents { get; init; }

    [Range(0, long.MaxValue)]
    public long PerKilometerCents { get; init; }

    [Range(0, long.MaxValue)]
    public long PerMinuteCents { get; init; }

    [Required]
    [RegularExpression("^[A-Z]{3}$")]
    public string Currency { get; init; } = string.Empty;

    public FarePolicy ToDomain() =>
        new(
            new Money(BaseFareCents, Currency),
            new Money(PerKilometerCents, Currency),
            new Money(PerMinuteCents, Currency));
}

public sealed class FarePolicyOptionsValidator : IValidateOptions<FarePolicyOptions>
{
    public ValidateOptionsResult Validate(string? name, FarePolicyOptions options)
    {
        var validationContext = new ValidationContext(options);
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateObject(options, validationContext, validationResults, validateAllProperties: true))
        {
            return ValidateOptionsResult.Success;
        }

        var failures = validationResults
            .SelectMany(result => result.MemberNames.Select(member => $"{member}: {result.ErrorMessage}"))
            .ToArray();

        return ValidateOptionsResult.Fail(failures);
    }
}
