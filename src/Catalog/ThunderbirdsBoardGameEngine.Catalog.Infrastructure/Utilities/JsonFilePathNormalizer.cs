using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities
{
    internal static partial class JsonFilePathNormalizer
    {
        private static readonly Regex _bracedVariable =
            BracedVariable();

        private static readonly Regex _dollarVariable =
            DollarVariable();

        public static string Normalize(string path, string contentRootPath)
        {
            var raw = path.Trim();

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
                    : Path.Combine(contentRootPath, raw);

                raw = Path.GetFullPath(absolute);
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is NotSupportedException ||
                ex is PathTooLongException ||
                ex is SecurityException)
            {
                raw = path; // revert to original if exception occurs
            }

            return raw;
        }

        private static string ExpandUnixEnvironmentTokens(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // ${VAR}
            var expanded = _bracedVariable.Replace(input,
                match => Environment.GetEnvironmentVariable(match.Groups["brace"].Value) ?? match.Value);

            // $VAR (avoid ${...} already handled)
            expanded = _dollarVariable.Replace(expanded,
                match => Environment.GetEnvironmentVariable(match.Groups["dollar"].Value) ?? match.Value);

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

        [GeneratedRegex(@"(?<!\$)\$(?<dollar>[A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
        private static partial Regex DollarVariable();

        [GeneratedRegex(@"\$\{(?<brace>[A-Za-z_][A-Za-z0-9_]*)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
        private static partial Regex BracedVariable();
    }
}
