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
            var cleaned = NormalizeWhitespace(input, nameof(input)).ToLowerInvariant();

            var stringBuilder = new StringBuilder();
            foreach (var c in cleaned)
            {
                if (char.IsLetterOrDigit(c))
                {
                    stringBuilder.Append(c);
                }
                else if (char.IsWhiteSpace(c) || c == '-' || c == '_')
                {
                    stringBuilder.Append('-');
                }
            }
            // Remove consecutive dashes
            var slug = stringBuilder.ToString();
            while (slug.Contains("--"))
            {
                slug = slug.Replace("--", "-");
            }
            // Trim leading/trailing dashes
            slug = slug.Trim('-');
            return slug;
        }
    }
}
