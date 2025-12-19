using MediatR;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class CalculateRescueTargetHandler : IRequestHandler<CalculateRescueTargetQuery, CalculateRescueTargetResponse>
    {
        private readonly IRescueProjectionProvider _rescueContextProvider;
        private readonly RescueTargetCalculator _bonusCalculator;

        public CalculateRescueTargetHandler(IRescueProjectionProvider rescueContextProvider, RescueTargetCalculator bonusCalculator)
        {
            _rescueContextProvider = rescueContextProvider;
            _bonusCalculator = bonusCalculator;
        }

        public Task<CalculateRescueTargetResponse> Handle(CalculateRescueTargetQuery request, CancellationToken cancellationToken)
        {
            var context = _rescueContextProvider.GetRescueContext(request.DisasterCardId);

            var calculatedTarget = _bonusCalculator.CalculateRescueTarget(request.AppliedBonusKeys, context);

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
