using System.Text;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers
{
    public static class StringHelpers
    {
        public static string NormalizeWhitespace(string input, string parameterName)
        {
            var cleaned = input.Trim().Normalize(NormalizationForm.FormC);

            if (string.IsNullOrWhiteSpace(cleaned))
            {
                return string.Empty;
            }

            if (cleaned.Any(char.IsControl))
            {
                throw new ArgumentException("String cannot contain control characters.", parameterName);
            }

            return cleaned;
        }

        public static string Slugify(string input)
        {
            var cleaned = NormalizeWhitespace(input, nameof(input))
                .ToLowerInvariant();

            var sb = new StringBuilder();
            bool lastWasDash = false;

            foreach (var c in cleaned)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                    lastWasDash = false;
                }
                else if (!lastWasDash)
                {
                    sb.Append('-');
                    lastWasDash = true;
                }
            }

            return sb.ToString().Trim('-');
        }
    }
}
