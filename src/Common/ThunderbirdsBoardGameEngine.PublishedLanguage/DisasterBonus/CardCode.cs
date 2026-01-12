using System.Text;
using System.Text.RegularExpressions;

namespace ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus
{
    /// <summary>
    /// Represents a normalized, validated code identifier for a card, enforcing a strict ASCII slug format.
    /// </summary>
    /// <remarks>A CardCode encapsulates a card's unique code in a canonical, lowercase, hyphen-separated
    /// format. The value is guaranteed to be non-empty, contain only lowercase ASCII letters, digits, and single
    /// hyphens as separators, and to have no leading, trailing, or consecutive hyphens. Use CardCode to ensure
    /// consistent handling and comparison of card codes throughout the application.</remarks>
    public readonly partial record struct CardCode
    {
        private static readonly Regex CodePattern =
            CodeSlugVariable();

        /// <summary>
        /// Gets the value represented by this instance.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the CardCode class with the specified code value.
        /// </summary>
        /// <param name="code">The card code to assign. Cannot be null or empty.</param>
        public CardCode(string code)
        {
            Value = NormalizeCode(code, nameof(code));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
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
