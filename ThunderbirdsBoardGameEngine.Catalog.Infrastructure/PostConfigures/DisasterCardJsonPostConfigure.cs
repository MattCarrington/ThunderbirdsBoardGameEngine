using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures
{
    internal class DisasterCardJsonPostConfigure : IPostConfigureOptions<DisasterCardJsonOptions>
    {
        private readonly IHostEnvironment _env;

        public DisasterCardJsonPostConfigure(IHostEnvironment env)
        {
            _env = env;
        }

        public void PostConfigure(string? name, DisasterCardJsonOptions options)
        {
            var raw = options.FilePath?.Trim();

            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new OptionsValidationException(
                    nameof(DisasterCardJsonOptions),
                    typeof(DisasterCardJsonOptions),
                    new[] { "CardData:FilePath is required." });
            }

            var combined = Path.IsPathRooted(raw) ? raw : Path.Combine(_env.ContentRootPath, raw);
            options.FilePath = Path.GetFullPath(combined);
        }
    }
}
