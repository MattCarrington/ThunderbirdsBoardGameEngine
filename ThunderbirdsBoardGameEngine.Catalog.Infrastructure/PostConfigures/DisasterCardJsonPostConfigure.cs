using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Security;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures
{
    internal sealed class DisasterCardJsonPostConfigure : IPostConfigureOptions<DisasterCardJsonOptions>
    {
        private readonly IHostEnvironment _env;

        public DisasterCardJsonPostConfigure(IHostEnvironment env)
        {
            _env = env;
        }

        public void PostConfigure(string? name, DisasterCardJsonOptions options)
        {
            var original = options.FilePath;

            if (string.IsNullOrWhiteSpace(original))
            {
                return; // return early so validation can catch the error
            }

            var raw = original.Trim();

            if (raw.Length >= 2)
            {
                var first = raw[0];
                var last = raw[^1];
                if ((first == '"' && last == '"') || (first == '\'' && last == '\''))
                {
                    raw = raw[1..^1];
                }
            }

            raw = Environment.ExpandEnvironmentVariables(raw);

            try
            {
                var absolute = Path.IsPathFullyQualified(raw)
                    ? raw
                    : Path.Combine(_env.ContentRootPath, raw);

                raw = Path.GetFullPath(absolute);
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is NotSupportedException ||
                ex is PathTooLongException ||
                ex is SecurityException)
            {
                raw = original; // revert to original if exception occurs
            }

            options.FilePath = raw;
        }
    }
}
