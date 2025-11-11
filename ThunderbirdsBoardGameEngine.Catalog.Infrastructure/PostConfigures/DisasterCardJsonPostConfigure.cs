using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures
{
    internal sealed partial class DisasterCardJsonPostConfigure : IPostConfigureOptions<DisasterCardJsonOptions>
    {
        private readonly IHostEnvironment _env;

        private static readonly Regex _bracedVariable =
            BracedVariable();

        private static readonly Regex _dollarVariable =
            DollarVariable();

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

            raw = ExpandUnixEnvironmentTokens(raw);

            raw = NormaliseSeparators(raw);

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

        private static string ExpandUnixEnvironmentTokens(string input)
        {
            if (string.IsNullOrEmpty(input)) 
            { 
                return input; 
            }

            // ${VAR}
            var expanded = _bracedVariable.Replace(input, 
                match => Environment.GetEnvironmentVariable(match.Groups["k"].Value) ?? match.Value);

            // $VAR (avoid ${...} already handled)
            expanded = _dollarVariable.Replace(expanded,
                match => Environment.GetEnvironmentVariable(match.Groups["k"].Value) ?? match.Value);

            return expanded;
        }

        private static string NormaliseSeparators(string input)
        {
            // Keep UNC prefix intact on Windows; only normalise '/' → '\'.
            // Do not touch leading backslashes or collapse them.

            if (string.IsNullOrEmpty(input)) 
            { 
                return input; 
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && input.StartsWith(@"\\"))
            {
                return input.Replace('/', '\\');
            }

            var sep = Path.DirectorySeparatorChar;

            return input.Replace('\\', sep).Replace('/', sep);
        }

        [GeneratedRegex(@"(?<!\$)\$(?<name>[A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
        private static partial Regex DollarVariable();

        [GeneratedRegex(@"\$\{(?<name>[A-Za-z_][A-Za-z0-9_]*)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
        private static partial Regex BracedVariable();
    }
}
