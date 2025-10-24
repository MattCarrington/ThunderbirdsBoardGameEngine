using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Application.Configuration;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Validators
{
    public class DisasterCardWarmupOptionsValidator : IValidateOptions<DisasterCardWarmupOptions>
    {
        public ValidateOptionsResult Validate(string? name, DisasterCardWarmupOptions options)
        {
            if (options is null)
            {
                return ValidateOptionsResult.Fail("DisasterCardWarmupOptions is required.");
            }

            if (!options.Enabled)
            {
                return ValidateOptionsResult.Success; // No further validation needed if disabled
            }

            if (options.Timeout <= TimeSpan.Zero)
            {
                return ValidateOptionsResult.Fail("DisasterCardWarmupOptions.Timeout must be greater than zero.");
            }

            if (options.Timeout > TimeSpan.FromMinutes(5))
            {
                return ValidateOptionsResult.Fail("DisasterCardWarmupOptions.Timeout must not exceed 5 minutes.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
