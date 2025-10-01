using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration
{
    internal class CardDataOptionsPostConfigure : IPostConfigureOptions<CardDataOptions>
    {
        private readonly IHostEnvironment _env;

        public CardDataOptionsPostConfigure(IHostEnvironment env)
        {
            _env = env;
        }

        public void PostConfigure(string? name, CardDataOptions options)
        {
            var raw = options.DisasterCardsFilePath?.Trim();

            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new OptionsValidationException(
                    nameof(CardDataOptions),
                    typeof(CardDataOptions),
                    new[] { "CardData:DisasterCardsFilePath is required." });
            }

            var combined = Path.IsPathRooted(raw) ? raw : Path.Combine(_env.ContentRootPath, raw);
            options.DisasterCardsFilePath = Path.GetFullPath(combined);
        }
    }
}
