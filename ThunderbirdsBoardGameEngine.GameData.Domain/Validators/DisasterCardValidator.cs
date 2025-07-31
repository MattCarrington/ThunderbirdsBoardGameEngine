using ThunderbirdsBoardGameEngine.GameData.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Domain.Validators
{
    public static class DisasterCardValidator
    {
        public static void ValidateAll(IEnumerable<DisasterCard> cards)
        {
            var ids = new HashSet<int>();

            foreach (var card in cards)
            {
                if (!ids.Add(card.Id))
                {
                    throw new DisasterCardValidationException($"Duplicate DisasterCard Id found: {card.Id}");
                }

                Validate(card);
            }
        }

        public static void Validate(DisasterCard card)
        {
            if (card.BonusConditions is null || !card.BonusConditions.Any())
            {
                throw new DisasterCardValidationException($"Disaster Card {card.Name} must have at least one bonus condition.");
            }

            if (card.RewardOptions is null || !card.RewardOptions.Any())
            {
                throw new DisasterCardValidationException($"Disaster Card {card.Name} must have at least one reward option.");
            }

            if (card.DifficultyNumber <= 0)
            {
                throw new DisasterCardValidationException($"Disaster Card {card.Name} has invalid DifficultyNumber: {card.DifficultyNumber}");
            }

            foreach (var reward in card.RewardOptions)
            {
                if (!reward.IsUserChoice && reward.SpecifiedToken is null)
                {
                    throw new DisasterCardValidationException($"Disaster Card {card.Name} has an invalid reward option. If User Choice is false, specified token must be provided");
                }
            }

            var seen = new HashSet<string>();

            foreach (var bonus in card.BonusConditions ?? Enumerable.Empty<BonusCondition>())
            {
                if (bonus.BonusValue <= 0)
                {
                    throw new DisasterCardValidationException($"Disaster Card {card.Name} has a bonus condition with invalid BonusValue: {bonus.BonusValue}");
                }

                var signature = GetBonusConditionSignature(bonus);
                if (!seen.Add(signature))
                {
                    throw new DisasterCardValidationException(
                        $"Card '{card.Name}' contains duplicate bonus condition: {signature}");
                }
            }
        }

        private static string GetBonusConditionSignature(BonusCondition bonus)
        {
            return bonus switch
            {
                CharacterBonusCondition cb => $"Character:{cb.Character}|Value:{cb.BonusValue}|Location:{cb.Location}",
                ThunderbirdBonusCondition tb => $"Thunderbird:{tb.Thunderbird}|Value:{tb.BonusValue}|Location:{tb.Location}",
                PodVehicleBonusCondition pb => $"PodVehicle:{pb.PodVehicle}|Value:{pb.BonusValue}|Location:{pb.Location}",
                _ => throw new DisasterCardValidationException($"Unknown bonus type: {bonus.GetType().Name}")
            };
        }

    }
}
