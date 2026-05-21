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
        private readonly ICalculateRescueTargetResolutionService _rescueCalculationService;

        public CalculateRescueTargetHandler(ICalculateRescueTargetResolutionService rescueCalculationService)
        {
            _rescueCalculationService = rescueCalculationService ?? throw new ArgumentNullException(nameof(rescueCalculationService));
        }

        public Task<CalculateRescueTargetResponse> Handle(CalculateRescueTargetQuery query, CancellationToken cancellationToken)
        {
            var request = new RescueCalculationRequest
            (
                DisasterCardCode: query.DisasterCardCode,
                PerformingCharacter: query.PerformingCharacter,
                PresentDisasterBonusKeys: query.PresentDisasterBonusKeys,
                PlayedFabCardCodes: query.PlayedFabCardCodes,
                ActiveEventCardCodes: query.ActiveEventCardCodes
            );

            var calculatedTarget = _rescueCalculationService.ResolveRescueCalculationAsync(request);

            return Task.FromResult(
                new CalculateRescueTargetResponse
                (
                    TargetNumber: calculatedTarget.TargetRoll,
                    TotalBonus: calculatedTarget.TotalBonus,
                    AppliedBonuses: calculatedTarget.AppliedModifiers
                )
            );
        }
    }
}
