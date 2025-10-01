using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration
{
    internal class CardDataOptionsFileValidator : IValidateOptions<CardDataOptions>
    {
        public ValidateOptionsResult Validate(string? name, CardDataOptions? options)
        {
            var path = options?.DisasterCardsFilePath?.Trim();

            if (string.IsNullOrWhiteSpace(path))
                return ValidateOptionsResult.Fail("CardData:DisasterCardsFilePath is required.");

            // Path has already been rooted/normalized by CardDataOptionsPostConfigure
            return File.Exists(path)
                ? ValidateOptionsResult.Success
                : ValidateOptionsResult.Fail($"Disaster cards file not found: {path}");
        }
    }
}
