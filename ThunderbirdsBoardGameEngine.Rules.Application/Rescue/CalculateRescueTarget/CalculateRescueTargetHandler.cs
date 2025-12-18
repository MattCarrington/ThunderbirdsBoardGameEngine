namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class CalculateRescueTargetHandler
    {
        private readonly IRescueProjectionProvider _rescueContextProvider;
        private readonly RescueTargetCalculator _bonusCalculator;

        public CalculateRescueTargetHandler(IRescueProjectionProvider rescueContextProvider, RescueTargetCalculator bonusCalculator)
        {
            _rescueContextProvider = rescueContextProvider;
            _bonusCalculator = bonusCalculator;
        }

        public CalculateRescueTargetResponse Handle(CalculateRescueTargetRequest request)
        {
            var context = _rescueContextProvider.GetRescueContext(request.DisasterCardId);

            var calculatedTarget = _bonusCalculator.CalculateRescueTarget(request.AppliedBonusKeys, context);

            return new CalculateRescueTargetResponse
            (
                TargetNumber: calculatedTarget.TargetRoll,
                TotalBonus: calculatedTarget.TotalBonus,
                AppliedBonuses: calculatedTarget.AppliedBonuses                    
            );
        }
    }
}
