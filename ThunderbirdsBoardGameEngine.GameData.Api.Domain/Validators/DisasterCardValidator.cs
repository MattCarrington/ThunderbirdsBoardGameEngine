using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Exceptions;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.Validators
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
            if (card.Bonuses is null || !card.Bonuses.Any())
            {
                throw new DisasterCardValidationException($"Disaster Card {card.Name} must have at least one bonus.");
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

            foreach (var bonus in card.Bonuses ?? Enumerable.Empty<Bonus>())
            {
                if (bonus.BonusValue <= 0)
                {
                    throw new DisasterCardValidationException($"Disaster Card {card.Name} has a bonus with invalid BonusValue: {bonus.BonusValue}");
                }

                var signature = GetBonusSignature(bonus);
                if (!seen.Add(signature))
                {
                    throw new DisasterCardValidationException(
                        $"Card '{card.Name}' contains duplicate bonus: {signature}");
                }
            }
        }

        private static string GetBonusSignature(Bonus bonus)
        {
            return bonus switch
            {
                CharacterBonus cb => $"Character:{cb.Character}|Value:{cb.BonusValue}|Location:{cb.Location}",
                ThunderbirdBonus tb => $"Thunderbird:{tb.Thunderbird}|Value:{tb.BonusValue}|Location:{tb.Location}",
                PodVehicleBonus pb => $"PodVehicle:{pb.PodVehicle}|Value:{pb.BonusValue}|Location:{pb.Location}",
                _ => throw new DisasterCardValidationException($"Unknown bonus type: {bonus.GetType().Name}")
            };
        }

    }
}
