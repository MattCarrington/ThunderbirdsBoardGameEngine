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
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.Description == y.Description
                && x.Key == y.Key;
        }

        /// <inheritdoc />
        public int GetHashCode(BonusConditionDto obj)
        {
            return obj.Description?.GetHashCode() ?? 0;
        }
    }
}
