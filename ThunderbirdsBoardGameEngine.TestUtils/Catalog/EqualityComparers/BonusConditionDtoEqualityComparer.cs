using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.EqualityComparers
{
    /// <summary>
    /// Defines equality for <see cref="BonusConditionDto"/> based on its description text.
    /// </summary>
    public sealed class BonusConditionDtoEqualityComparer : IEqualityComparer<BonusConditionDto>
    {
        /// <inheritdoc />
        public static readonly BonusConditionDtoEqualityComparer Instance = new();

        /// <inheritdoc />
        public bool Equals(BonusConditionDto? x, BonusConditionDto? y)
        {
            return string.Equals(x?.Description, y?.Description, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public int GetHashCode(BonusConditionDto obj)
        {
            return obj.Description?.GetHashCode() ?? 0;
        }
    }
}
