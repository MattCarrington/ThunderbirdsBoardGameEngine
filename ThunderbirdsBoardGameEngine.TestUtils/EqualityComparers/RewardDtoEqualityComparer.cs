using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.TestUtils.EqualityComparers
{
    public sealed class RewardDtoEqualityComparer : IEqualityComparer<RewardDto>
    {
        public static readonly RewardDtoEqualityComparer Instance = new();

        public bool Equals(RewardDto? x, RewardDto? y)
        {
            return string.Equals(x?.DisplayName, y?.DisplayName, StringComparison.Ordinal);
        }

        public int GetHashCode(RewardDto obj)
        {
            return obj.DisplayName?.GetHashCode() ?? 0;
        }
    }
}
