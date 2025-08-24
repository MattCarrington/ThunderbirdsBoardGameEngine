using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.TestUtils.EqualityComparers
{
    public sealed class DisasterCardDtoEqualityComparer : IEqualityComparer<DisasterCardDto>
    {
        public static readonly DisasterCardDtoEqualityComparer Instance = new();

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

            return x.Id == y.Id
                && x.Name == y.Name
                && x.DifficultyNumber == y.DifficultyNumber
                && x.Location == y.Location
                && x.RescueType == y.RescueType
                && x.BonusConditions.SequenceEqual(y.BonusConditions, BonusConditionDtoEqualityComparer.Instance)
                && x.Rewards.SequenceEqual(y.Rewards, RewardDtoEqualityComparer.Instance);
        }

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
