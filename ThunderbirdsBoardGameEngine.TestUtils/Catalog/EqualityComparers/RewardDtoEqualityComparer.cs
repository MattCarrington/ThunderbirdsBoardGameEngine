using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.EqualityComparers
{
    /// <summary>
    /// Defines equality for <see cref="RewardDto"/> based on its display name.
    /// </summary>
    public sealed class RewardDtoEqualityComparer : IEqualityComparer<RewardDto>
    {
        /// <inheritdoc />
        public static readonly RewardDtoEqualityComparer Instance = new();

        /// <inheritdoc />
        public bool Equals(RewardDto? x, RewardDto? y)
        {
            return string.Equals(x?.DisplayName, y?.DisplayName, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public int GetHashCode(RewardDto obj)
        {
            return obj.DisplayName?.GetHashCode() ?? 0;
        }
    }
}
