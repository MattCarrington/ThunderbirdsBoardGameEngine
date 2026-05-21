using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class CalculateRescueTargetResolutionService : ICalculateRescueTargetResolutionService
    {
        private readonly IDisasterContributionLookup _disasterContributionLookup;
        private readonly ICharacterContributionLookup _characterContributionLookup;
        private readonly IBonusModifierSourceRegistry _bonusModifierSourceRegistry;
        private readonly RescueTargetCalculator _rescueTargetCalculator;

        public CalculateRescueTargetResolutionService(
            IDisasterContributionLookup disasterContributionLookup,
            ICharacterContributionLookup characterContributionLookup,
            IBonusModifierSourceRegistry bonusModifierSourceRegistry,
            RescueTargetCalculator rescueTargetCalculator)
        {
            _disasterContributionLookup = disasterContributionLookup ?? throw new ArgumentNullException(nameof(disasterContributionLookup));
            _characterContributionLookup = characterContributionLookup ?? throw new ArgumentNullException(nameof(characterContributionLookup));
            _bonusModifierSourceRegistry = bonusModifierSourceRegistry ?? throw new ArgumentNullException(nameof(bonusModifierSourceRegistry));
            _rescueTargetCalculator = rescueTargetCalculator ?? throw new ArgumentNullException(nameof(rescueTargetCalculator));
        }

        public RescueTargetResult ResolveRescueCalculationAsync(
            RescueCalculationRequest request)
        {
            var disaster = _disasterContributionLookup.GetDisasterContribution(request.DisasterCardCode);

            var character = _characterContributionLookup.GetCharacterContribution(request.PerformingCharacter);

            var input = new RescueCalculationInput(request.PresentDisasterBonusKeys, disaster.RescueType);

            var sources = new List<IRescueModifierSource>
            {
                disaster,
                character
            };

            foreach (var fabCardCode in request.PlayedFabCardCodes)
            {
                if (_bonusModifierSourceRegistry.TryGetBonusModifierSource(fabCardCode, out var fabCard))
                {
                    sources.Add(fabCard);
                }
            }

            foreach (var eventCardCode in request.ActiveEventCardCodes)
            {
                if (_bonusModifierSourceRegistry.TryGetBonusModifierSource(eventCardCode, out var eventCard))
                {
                    sources.Add(eventCard);
                }
            }

            return _rescueTargetCalculator.CalculateRescueTarget(disaster.DifficultyNumber, input, sources);
        }
    }
}
