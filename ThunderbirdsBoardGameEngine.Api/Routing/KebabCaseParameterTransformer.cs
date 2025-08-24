using System.Text.RegularExpressions;

namespace ThunderbirdsBoardGameEngine.Api.Routing
{
    public class KebabCaseParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;

            // DisasterCards -> disaster-cards
            return Regex.Replace(
                value.ToString()!,
                "([a-z])([A-Z])",
                "$1-$2",
                RegexOptions.CultureInvariant,
                TimeSpan.FromMilliseconds(100))
                .ToLowerInvariant();
        }
    }
}
