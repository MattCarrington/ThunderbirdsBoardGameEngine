using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.EqualityComparers
{
    /// <summary>
    /// Defines domain-specific equality for <see cref="DisasterCardDto"/> instances,
    /// comparing identity, core properties, and child collections rather than relying on
    /// the record's default structural equality.
    /// </summary>
    public sealed class DisasterCardDtoEqualityComparer : IEqualityComparer<DisasterCardDto>
    {
        /// <inheritdoc />
        public static readonly DisasterCardDtoEqualityComparer Instance = new();

        /// <inheritdoc />
        public bool Equals(DisasterCardDto? x, DisasterCardDto? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            var xBonusConditions = x.BonusConditions ?? Array.Empty<BonusConditionDto>();
            var yBonusConditions = y.BonusConditions ?? Array.Empty<BonusConditionDto>();

            var xRewards = x.Rewards ?? Array.Empty<RewardDto>();
            var yRewards = y.Rewards ?? Array.Empty<RewardDto>();

            return x.Id == y.Id
                && x.Name == y.Name
                && x.DifficultyNumber == y.DifficultyNumber
                && x.Location == y.Location
                && x.RescueType == y.RescueType
                && xBonusConditions.SequenceEqual(yBonusConditions, BonusConditionDtoEqualityComparer.Instance)
                && xRewards.SequenceEqual(yRewards, RewardDtoEqualityComparer.Instance)
                && x.Code == y.Code;
        }

        /// <inheritdoc />
        public int GetHashCode([DisallowNull] DisasterCardDto obj)
        {
            var hash = new HashCode();
            hash.Add(obj.Id);
            hash.Add(obj.Name);
            hash.Add(obj.DifficultyNumber);
            hash.Add(obj.Location);
            hash.Add(obj.RescueType);

            foreach (var bc in obj.BonusConditions)
            {
                hash.Add(BonusConditionDtoEqualityComparer.Instance.GetHashCode(bc));
            }

            foreach (var r in obj.Rewards)
            {
                hash.Add(RewardDtoEqualityComparer.Instance.GetHashCode(r));
            }

            return hash.ToHashCode();
        }
    }
}
