using System.Text;
using System.Text.RegularExpressions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public partial class DisasterCard
    {
        private static readonly Regex CodePattern =
            CodeSlugVariable();

        public int Id { get; }

        public string Name { get; }

        public string Code { get; }

        public int DifficultyNumber { get; }

        public BoardLocation Location { get; }

        public RescueType RescueType { get; }

        public IReadOnlyList<BonusCondition> BonusConditions => _bonus.AsReadOnly();

        public IReadOnlyList<RewardOption> RewardOptions => _rewards.AsReadOnly();

        private readonly List<BonusCondition> _bonus;
        private readonly List<RewardOption> _rewards;

        public DisasterCard(int id, string name, string code, int difficultyNumber, BoardLocation location, RescueType rescueType,
            IEnumerable<BonusCondition> bonusConditions, IEnumerable<RewardOption> rewardOptions)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

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
            Code = NormalizeCode(code, nameof(code));
            Name = NormalizeString(name, nameof(name));
            DifficultyNumber = difficultyNumber;
            Location = location;
            RescueType = rescueType;
            _bonus = bonuses;
            _rewards = rewards;
        }

        private static string NormalizeString(string? input, string parameterName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(input);

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

        private static string NormalizeCode(string? code, string paramName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            var trimmed = code.Trim();

            // 1. Reject control characters outright
            if (trimmed.Any(char.IsControl))
            {
                throw new ArgumentException("Invalid code format: contains control characters.", paramName);
            }

            // 2. Reject illegal characters (anything outside safe ASCII set)
            if (trimmed.Any(ch => ch > 127))
            {
                throw new ArgumentException("Invalid code format: contains non-ASCII characters.", paramName);
            }

            // 3. Reject leading/trailing hyphens or double hyphens BEFORE collapsing
            if (trimmed.StartsWith('-') || trimmed.EndsWith('-') || trimmed.Contains("--"))
            {
                throw new ArgumentException("Invalid code format: leading/trailing/double hyphen.", paramName);
            }

            // 4. Reject any illegal symbol (skip space, dash, underscore, slash, dot)
            foreach (var ch in trimmed)
            {
                if (!char.IsLetterOrDigit(ch) &&
                    !" -_./".Contains(ch))
                {
                    throw new ArgumentException($"Invalid code format: contains illegal character '{ch}'.", paramName);
                }
            }

            // 5. Proceed with canonical normalization
            var s = trimmed.ToLowerInvariant().Normalize(NormalizationForm.FormKD);

            var sb = new StringBuilder(s.Length);
            bool dash = false;

            foreach (var ch in s)
            {
                if (ch is >= 'a' and <= 'z' or >= '0' and <= '9')
                {
                    sb.Append(ch);
                    dash = false;
                }
                else if ((char.IsWhiteSpace(ch) || ch is '-' or '_' or '/' or '.') && !dash)
                {
                    sb.Append('-');
                    dash = true;
                }
            }

            var normalized = sb.ToString().Trim('-');

            if (!CodePattern.IsMatch(normalized))
            {
                throw new ArgumentException($"Invalid code format: '{normalized}'", paramName);
            }

            return normalized;
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
                            throw new ArgumentException($"Duplicate bonus for character {characterBonus.Character} found.", nameof(bonusConditions));
                        }
                        break;
                    case ThunderbirdBonusCondition thunderbirdBonus:
                        if (!seenThunderbirds.Add(thunderbirdBonus.Thunderbird))
                        {
                            throw new ArgumentException($"Duplicate bonus for Thunderbird {thunderbirdBonus.Thunderbird} found.", nameof(bonusConditions));
                        }
                        break;
                    case PodVehicleBonusCondition podVehicleBonus:
                        if (!seenPodVehicles.Add(podVehicleBonus.PodVehicle))
                        {
                            throw new ArgumentException($"Duplicate bonus for Pod Vehicle {podVehicleBonus.PodVehicle} found.", nameof(bonusConditions));
                        }
                        break;
                    default:
                        throw new ArgumentException(
                            $"Unknown bonus condition type: {bonus?.GetType().Name ?? "<null>"}",
                            nameof(bonusConditions));
                }
            }
        }

        [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled)]
        private static partial Regex CodeSlugVariable();
    }
}
