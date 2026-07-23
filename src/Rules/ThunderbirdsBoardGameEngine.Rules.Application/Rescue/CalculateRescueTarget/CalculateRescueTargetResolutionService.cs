using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class CalculateRescueTargetResolutionService : ICalculateRescueTargetResolutionService
    {
        private readonly IDisasterCatalogLookup _disasterCatalogLookup;
        private readonly ICharacterCatalogLookup _characterCatalogLookup;
        private readonly IFabCardCatalogLookup _fabCardCatalogLookup;
        private readonly IEventCardCatalogLookup _eventCardCatalogLookup;
        private readonly ICardBonusModifierSourceRegistry _bonusModifierSourceRegistry;
        private readonly RescueTargetCalculator _rescueTargetCalculator;

        public CalculateRescueTargetResolutionService(
            IDisasterCatalogLookup disasterCatalogLookup,
            ICharacterCatalogLookup characterCatalogLookup,
            IFabCardCatalogLookup fabCardCatalogLookup,
            IEventCardCatalogLookup eventCardCatalogLookup,
            ICardBonusModifierSourceRegistry bonusModifierSourceRegistry,
            RescueTargetCalculator rescueTargetCalculator)
        {
            _disasterCatalogLookup = disasterCatalogLookup ?? throw new ArgumentNullException(nameof(disasterCatalogLookup));
            _characterCatalogLookup = characterCatalogLookup ?? throw new ArgumentNullException(nameof(characterCatalogLookup));
            _fabCardCatalogLookup = fabCardCatalogLookup ?? throw new ArgumentNullException(nameof(fabCardCatalogLookup));
            _eventCardCatalogLookup = eventCardCatalogLookup ?? throw new ArgumentNullException(nameof(eventCardCatalogLookup));
            _bonusModifierSourceRegistry = bonusModifierSourceRegistry ?? throw new ArgumentNullException(nameof(bonusModifierSourceRegistry));
            _rescueTargetCalculator = rescueTargetCalculator ?? throw new ArgumentNullException(nameof(rescueTargetCalculator));
        }

        public RescueTargetResult ResolveRescueCalculation(RescueCalculationRequest request)
        {
            var disaster = _disasterCatalogLookup.GetDisasterRescueContribution(request.DisasterCardCode);

            var character = _characterCatalogLookup.GetCharacterRescueContribution(request.PerformingCharacter);

            var input = new RescueCalculationInput(request.PresentDisasterBonusKeys, disaster.RescueType);

            var sources = new List<IRescueModifierSource>
            {
                disaster,
                character
            };

            foreach (var fabCard in request.PlayedFabCardCodes)
            {
                if (!_fabCardCatalogLookup.Exists(fabCard))
                {
                    throw new InvalidRescueCalculationRequestException("F.A.B. Card", fabCard.ToString());
                }
            }

            foreach (var eventCard in request.ActiveEventCardCodes)
            {
                if (!_eventCardCatalogLookup.Exists(eventCard))
                {
                    throw new InvalidRescueCalculationRequestException("Event Card", eventCard.ToString());
                }
            }

            var cardCodes = request.PlayedFabCardCodes
                .Concat(request.ActiveEventCardCodes)
                .Distinct();

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
