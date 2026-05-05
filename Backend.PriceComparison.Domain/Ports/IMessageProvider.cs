namespace Backend.PriceComparison.Domain.Ports;

/// <summary>
/// Localized message catalog used by validators and error builders.
/// Implementations resolve each property against an external resource file
/// (for example <c>MessageProviderResource.resx</c>) so that messages can
/// be translated without changing the domain.
/// </summary>
public interface IMessageProvider
{
    /// <summary>"Field cannot be null" validation message.</summary>
    string ErrorValidatorFieldNotNull { get; }

    /// <summary>"Field cannot be empty" validation message.</summary>
    string ErrorValidatorFieldNotEmpty { get; }

    /// <summary>"Field must be greater than zero" validation message.</summary>
    string ErrorValidatorFieldGreatherThanZero { get; }

    /// <summary>"Field must be less than twelve" validation message.</summary>
    string ErrorValidatorFieldLessThanTwelve { get; }

    /// <summary>"Resolution type is not valid" validation message.</summary>
    string ErrorValidatorResolutionTypeValid { get; }

    /// <summary>"Field must be positive" validation message.</summary>
    string ErrorValidatorFieldPositive { get; }

    /// <summary>"Field must be greater than zero" validation message (alternate wording).</summary>
    string ErrorValidatorFieldMustBeGreaterThanZero { get; }

    /// <summary>"Field must be non-negative" validation message.</summary>
    string ErrorValidatorFieldMustBeNonNegative { get; }

    /// <summary>"Field must be a percentage" validation message.</summary>
    string ErrorValidatorFieldMustBePercentage { get; }

    /// <summary>"Invalid email" validation message.</summary>
    string ErrorValidatorInvalidEmail { get; }

    /// <summary>"Field must be greater than or equal to zero" validation message.</summary>
    string ErrorValidatorFieldMustBeGreaterThanOrEqualToZero { get; }

    /// <summary>"Field is required" validation message.</summary>
    string ErrorValidatorFieldIsRequired { get; }
}
