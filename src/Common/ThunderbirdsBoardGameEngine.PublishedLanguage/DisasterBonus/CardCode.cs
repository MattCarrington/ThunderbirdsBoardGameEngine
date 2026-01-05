using System.Text;
using System.Text.RegularExpressions;

namespace ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus
{
    public readonly partial record struct CardCode
    {
        private static readonly Regex CodePattern =
            CodeSlugVariable();

        public string Value { get; }

        public CardCode(string code)
        {
            Value = NormalizeCode(code, nameof(code));
        }

        public override string ToString()
        {
            return Value;
        }

        private static string NormalizeCode(string? code, string paramName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            var trimmed = code.Trim();

            // 1. Reject control characters outright
            if (trimmed.Any(char.IsControl))
            {
                throw new ArgumentException("Invalid code format: contains control characters.", paramName);
            }

            // 2. Reject illegal characters (anything outside safe ASCII set)
            if (trimmed.Any(ch => ch > 127))
            {
                throw new ArgumentException("Invalid code format: contains non-ASCII characters.", paramName);
            }

            // 3. Reject leading/trailing hyphens or double hyphens BEFORE collapsing
            if (trimmed.StartsWith('-') || trimmed.EndsWith('-') || trimmed.Contains("--"))
            {
                throw new ArgumentException("Invalid code format: leading/trailing/double hyphen.", paramName);
            }

            // 4. Reject any illegal symbol (skip space, dash, underscore, slash, dot)
            foreach (var ch in trimmed)
            {
                if (!char.IsLetterOrDigit(ch) &&
                    !" -_./".Contains(ch))
                {
                    throw new ArgumentException($"Invalid code format: contains illegal character '{ch}'.", paramName);
                }
            }

            // 5. Proceed with canonical normalization
            var s = trimmed.ToLowerInvariant().Normalize(NormalizationForm.FormKD);

            var sb = new StringBuilder(s.Length);
            bool dash = false;

            foreach (var ch in s)
            {
                if (ch is >= 'a' and <= 'z' or >= '0' and <= '9')
                {
                    sb.Append(ch);
                    dash = false;
                }
                else if ((char.IsWhiteSpace(ch) || ch is '-' or '_' or '/' or '.') && !dash)
                {
                    sb.Append('-');
                    dash = true;
                }
            }

            var normalized = sb.ToString().Trim('-');

            if (!CodePattern.IsMatch(normalized))
            {
                throw new ArgumentException($"Invalid code format: '{normalized}'", paramName);
            }

            return normalized;
        }

        [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled)]
        private static partial Regex CodeSlugVariable();
    }
}
