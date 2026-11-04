using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    internal sealed class NoBlankItemsAttribute : ValidationAttribute
    {
        public NoBlankItemsAttribute()
            : base("{0} cannot contain null or whitespace values.")
        {
        }

        public override bool IsValid(object? value)
        {
            // Leave null validation to [Required].
            if (value is null)
            {
                return true;
            }

            return value is IEnumerable<string> items
                && !items.Any(string.IsNullOrWhiteSpace);
        }
    }
}
