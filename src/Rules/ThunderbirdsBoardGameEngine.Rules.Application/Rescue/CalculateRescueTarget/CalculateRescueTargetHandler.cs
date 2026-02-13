using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    /// <summary>
    /// Handles rescue target calculation requests by orchestrating rule evaluation.
    /// </summary>
    /// <remarks>
    /// This handler is responsible for:
    /// - Constructing the set of applicable bonus modifier sources (including the disaster and character contributions)
    /// - Delegating bonus evaluation to the rules engine
    ///
    /// The handler enforces core invariants (valid disaster and character) but does not
    /// validate board state, action legality, or bonus key relevance.
    /// </remarks>
    public class CalculateRescueTargetHandler : IRequestHandler<CalculateRescueTargetQuery, CalculateRescueTargetResponse>
    {
        private readonly IDisasterContributionLookup _disasterContributionLookup;
        private readonly ICharacterContributionLookup _characterContributionLookup;
        private readonly RescueTargetCalculator _rescueTargetCalculator;

        public CalculateRescueTargetHandler(
            IDisasterContributionLookup disasterContributionLookup,
            ICharacterContributionLookup characterContributionLookup,
            RescueTargetCalculator rescueTargetCalculator)
        {
            _disasterContributionLookup = disasterContributionLookup ?? throw new ArgumentNullException(nameof(disasterContributionLookup));
            _characterContributionLookup = characterContributionLookup ?? throw new ArgumentNullException(nameof(characterContributionLookup));
            _rescueTargetCalculator = rescueTargetCalculator ?? throw new ArgumentNullException(nameof(rescueTargetCalculator));
        }

        public Task<CalculateRescueTargetResponse> Handle(CalculateRescueTargetQuery query, CancellationToken cancellationToken)
        {
            var disaster = _disasterContributionLookup.GetDisasterContribution(query.DisasterCardCode);

            var character = _characterContributionLookup.GetCharacterContribution(query.PerformingCharacter);

            var input = new RescueCalculationInput(query.PresentDisasterBonusKeys, disaster.RescueType);

            var sources = new List<IBonusModifierSource>
            {
                disaster,
                character
            };

            var calculatedTarget = _rescueTargetCalculator.CalculateRescueTarget(disaster.DifficultyNumber, input, sources);

            return Task.FromResult(
                new CalculateRescueTargetResponse
                (
                    TargetNumber: calculatedTarget.TargetRoll,
                    TotalBonus: calculatedTarget.TotalBonus,
                    AppliedBonuses: calculatedTarget.AppliedBonuses
                )
            );
        }
    }
}
