using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.TestUtils.EqualityComparers
{
    public sealed class BonusConditionDtoEqualityComparer : IEqualityComparer<BonusConditionDto>
    {
        public static readonly BonusConditionDtoEqualityComparer Instance = new();

        public bool Equals(BonusConditionDto? x, BonusConditionDto? y) =>
            string.Equals(x?.Description, y?.Description, StringComparison.Ordinal);

        public int GetHashCode(BonusConditionDto obj) => obj.Description?.GetHashCode() ?? 0;
    }
}
