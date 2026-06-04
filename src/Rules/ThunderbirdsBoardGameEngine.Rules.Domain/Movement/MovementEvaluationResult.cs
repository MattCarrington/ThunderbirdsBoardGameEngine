using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed record MovementEvaluationResult(
        bool IsMoveValid,
        IReadOnlyCollection<LocationCode> Route,
        int SpacesTravelled,
        int TopSpeed,
        int ActionPointCost,
        IReadOnlyCollection<string> Messages)
    {
        public static MovementEvaluationResult InvalidMove(
            string message)
        {
            return new(
                IsMoveValid: false,
                Route: Array.Empty<LocationCode>(),
                SpacesTravelled: 0,
                TopSpeed: 0,
                ActionPointCost: 0,
                Messages: new[] { message });
        }

        public static MovementEvaluationResult ValidMove(
            IReadOnlyCollection<LocationCode> route,
            int spacesTravelled,
            int topSpeed,
            int actionPointCost)
        {
            return new(
                IsMoveValid: true,
                Route: route,
                SpacesTravelled: spacesTravelled,
                TopSpeed: topSpeed,
                ActionPointCost: actionPointCost,
                Messages: Array.Empty<string>());
        }
    }
}
