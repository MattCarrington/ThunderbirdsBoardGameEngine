using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class CalculateRescueTargetResolutionService : ICalculateRescueTargetResolutionService
    {
        private readonly IDisasterCatalogLookup _disasterContributionLookup;
        private readonly ICharacterCatalogLookup _characterContributionLookup;
        private readonly IBonusModifierSourceRegistry _bonusModifierSourceRegistry;
        private readonly RescueTargetCalculator _rescueTargetCalculator;

        public CalculateRescueTargetResolutionService(
            IDisasterCatalogLookup disasterContributionLookup,
            ICharacterCatalogLookup characterContributionLookup,
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
            var disaster = _disasterContributionLookup.GetDisasterRescueContribution(request.DisasterCardCode);

            var character = _characterContributionLookup.GetCharacterRescueContribution(request.PerformingCharacter);

            var input = new RescueCalculationInput(request.PresentDisasterBonusKeys, disaster.RescueType);

            var sources = new List<IRescueModifierSource>
            {
                disaster,
                character
            };

            var cardCodes = request.PlayedFabCardCodes.Concat(request.ActiveEventCardCodes);

            foreach (var cardCode in cardCodes)
            {
                if (_bonusModifierSourceRegistry.TryGetBonusModifierSource(cardCode, out var source))
                {
                    sources.Add(source);
                }
            }

            return _rescueTargetCalculator.CalculateRescueTarget(disaster.DifficultyNumber, input, sources);
        }
    }
}
