using System.Text;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public class DisasterCard
    {
        public int Id { get; }

        public string Name { get; }

        public int DifficultyNumber { get; }

        public BoardLocation Location { get; }

        public RescueType RescueType { get; }

        public IReadOnlyList<BonusCondition> BonusConditions => _bonus.AsReadOnly();

        public IReadOnlyList<RewardOption> RewardOptions => _rewards.AsReadOnly();

        private readonly List<BonusCondition> _bonus;
        private readonly List<RewardOption> _rewards;

        [JsonConstructor]
        public DisasterCard(int id, string name, int difficultyNumber, BoardLocation location, RescueType rescueType,
            IEnumerable<BonusCondition> bonusConditions, IEnumerable<RewardOption> rewardOptions)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(difficultyNumber);
                
            ArgumentNullException.ThrowIfNull(bonusConditions);
            ArgumentNullException.ThrowIfNull(rewardOptions);

            var bonuses = bonusConditions.ToList();
            var rewards = rewardOptions.ToList();

            ArgumentOutOfRangeException.ThrowIfLessThan(bonuses.Count, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(rewards.Count, 1);

            if (bonuses.Any(b => b is null))
            {
                throw new ArgumentException("Bonus conditions must not contain null entries.", nameof(bonusConditions));
            }

            if (rewards.Any(r => r is null))
            {
                throw new ArgumentException("Reward options must not contain null entries.", nameof(rewardOptions));
            }

            EnsureUniqueBonusConditions(bonuses);

            Id = id;
            Name = NormalizeString(name, nameof(name));
            DifficultyNumber = difficultyNumber;
            Location = location;
            RescueType = rescueType;
            _bonus = bonuses;
            _rewards = rewards;
        }

        private static string NormalizeString(string? input, string parameterName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(input, nameof(input));

            var cleaned = input.Trim().Normalize(NormalizationForm.FormC);

            if (cleaned.Length == 0) 
            {
                throw new ArgumentException("String cannot be empty or whitespace after cleaning.", parameterName);
            }

            if (cleaned.Any(char.IsControl))
            {
                throw new ArgumentException("String cannot contain control characters.", parameterName);
            }

            return cleaned;
        }

        private static void EnsureUniqueBonusConditions(IEnumerable<BonusCondition> bonusConditions)
        {
            var seenCharacters = new HashSet<Character>();
            var seenThunderbirds = new HashSet<ThunderbirdMachine>();
            var seenPodVehicles = new HashSet<PodVehicle>();

            foreach (var bonus in bonusConditions)
            {
                switch (bonus)
                {
                    case CharacterBonusCondition characterBonus:
                        if (!seenCharacters.Add(characterBonus.Character))
                        {
                            throw new ArgumentException($"Duplicate bonus bonus for character {characterBonus.Character} found.", nameof(bonusConditions));
                        }
                        break;
                    case ThunderbirdBonusCondition thunderbirdBonus:
                        if (!seenThunderbirds.Add(thunderbirdBonus.Thunderbird))
                        {
                            throw new ArgumentException($"Duplicate bonus bonus for Thunderbird {thunderbirdBonus.Thunderbird} found.", nameof(bonusConditions));
                        }
                        break;
                    case PodVehicleBonusCondition podVehicleBonus:
                        if (!seenPodVehicles.Add(podVehicleBonus.PodVehicle))
                        {
                            throw new ArgumentException($"Duplicate bonus bonus for Pod Vehicle {podVehicleBonus.PodVehicle} found.", nameof(bonusConditions));
                        }
                        break;
                    default:
                        throw new ArgumentException(
                            $"Unknown bonus condition type: {bonus?.GetType().Name ?? "<null>"}",
                            nameof(bonusConditions));
                }
            }
        }
    }
}
