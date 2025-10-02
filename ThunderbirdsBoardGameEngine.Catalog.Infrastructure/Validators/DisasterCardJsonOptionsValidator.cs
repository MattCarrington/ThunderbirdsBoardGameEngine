using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators
{
    internal class DisasterCardJsonOptionsValidator : IValidateOptions<DisasterCardJsonOptions>
    {
        public ValidateOptionsResult Validate(string? name, DisasterCardJsonOptions? options)
        {
            var path = options?.FilePath?.Trim();

            if (string.IsNullOrWhiteSpace(path))
                return ValidateOptionsResult.Fail("CardData:FilePath is required.");

            // Path has already been rooted/normalized by CardDataOptionsPostConfigure
            return File.Exists(path)
                ? ValidateOptionsResult.Success
                : ValidateOptionsResult.Fail($"Disaster cards file not found: {path}");
        }
    }
}
