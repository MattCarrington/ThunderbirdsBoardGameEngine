using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Routing;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation
{
    public sealed class MovementEvaluator
    {
        private readonly IRouteFinder _routeFinder;
        private readonly IMovementSpeedModifierSourceRegistry _speedRegistry;
        private readonly IEffectiveTopographyResolver _topographyResolver;
        private readonly ActionPointCalculator _actionPointCalculator;

        public MovementEvaluator(
            IRouteFinder routeFinder,
            IMovementSpeedModifierSourceRegistry speedRegistry,
            IEffectiveTopographyResolver topographyResolver,
            ActionPointCalculator actionPointCalculator)
        {
            _routeFinder = routeFinder ?? throw new ArgumentNullException(nameof(routeFinder));
            _speedRegistry = speedRegistry ?? throw new ArgumentNullException(nameof(speedRegistry));
            _topographyResolver = topographyResolver ?? throw new ArgumentNullException(nameof(topographyResolver));
            _actionPointCalculator = actionPointCalculator ?? throw new ArgumentNullException(nameof(actionPointCalculator));
        }

        public MovementEvaluationResult Evaluate(MovementEvaluationInput input)
        {
            if (input.Thunderbird.TopSpeed <= 0)
            {
                return MovementEvaluationResult.InvalidMove(
                    $"{input.Thunderbird.Key.Value} cannot move.");
            }

            var effectiveTopography = _topographyResolver.Resolve(input.Topography, input.EventCards);
            var routeInput = input with { Topography = effectiveTopography.Value };

            var route = _routeFinder.FindShortestRoute(routeInput);

            if (route is null)
            {
                return MovementEvaluationResult.InvalidMove(
                    $"No route found from {input.Start.Value} to {input.Destination.Value} for {input.Thunderbird.Key.Value}.");
            }

            AppliedMovementSpeedModifier? applicableModifier = null;

            foreach (var eventCard in input.EventCards)
            {
                if (!_speedRegistry.TryGetEventCard(eventCard, out var source))
                {
                    continue;
                }

                applicableModifier = source.ApplyMovementModifier(input.Thunderbird.Key);

                if (applicableModifier is not null)
                {
                    break;
                }
            }

            var effectiveTopSpeed = applicableModifier?.EffectiveTopSpeed ?? input.Thunderbird.TopSpeed;

            var actionPointCost = _actionPointCalculator.CalculateActionPoints(
                route.SpacesTravelled,
                effectiveTopSpeed);

            return MovementEvaluationResult.ValidMove(
                route: route.Route,
                spacesTravelled: route.SpacesTravelled,
                topSpeed: effectiveTopSpeed,
                actionPointCost: actionPointCost,
                messages:
                [
                    .. effectiveTopography.Messages,
                    .. applicableModifier is null ? [] : new[] { applicableModifier.Message }
                ]);
        }
    }
}
