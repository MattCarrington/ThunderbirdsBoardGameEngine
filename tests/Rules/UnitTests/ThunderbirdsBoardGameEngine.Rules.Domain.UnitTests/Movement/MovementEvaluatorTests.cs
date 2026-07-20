using NSubstitute;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Movement
{
    public class MovementEvaluatorTests
    {
        [Fact]
        public void Evaluate_WhenRouteExists_ReturnsValidMove()
        {
            // Arrange
            var input = CreateMovementInput(3);

            var route = new[] { new LocationCode("A"), new LocationCode("C"), new LocationCode("B") };

            var routeFinder = Substitute.For<IRouteFinder>();
            routeFinder.FindShortestRoute(Arg.Any<MovementInput>()).Returns(new RouteResult
            (
                Route: route.ToList(),
                SpacesTravelled: 2
            ));

            var movementEvaluator = CreateMovementEvaluator(routeFinder);

            // Act
            var result = movementEvaluator.Evaluate(input);

            // Assert
            Assert.True(result.IsMoveValid);
            Assert.Equal(route, result.Route);
            Assert.Equal(2, result.SpacesTravelled);
            Assert.Equal(3, result.TopSpeed);
            Assert.Equal(1, result.ActionPointCost);
            Assert.Empty(result.Messages);
        }

        [Fact]
        public void Evaluate_WhenNoRouteExists_ReturnsInvalidMove()
        {
            // Arrange
            var input = CreateMovementInput(3);

            var routeFinder = Substitute.For<IRouteFinder>();
            routeFinder.FindShortestRoute(Arg.Any<MovementInput>()).Returns((RouteResult?)null);

            var movementEvaluator = CreateMovementEvaluator(routeFinder);

            // Act
            var result = movementEvaluator.Evaluate(input);

            // Assert
            Assert.False(result.IsMoveValid);
            Assert.Empty(result.Route);
            Assert.Equal(0, result.SpacesTravelled);
            Assert.Equal(0, result.TopSpeed);
            Assert.Equal(0, result.ActionPointCost);
            Assert.Single(result.Messages);
            Assert.Equal("No route found from A to B for thunderbird.", result.Messages.First());
        }

        [Fact]
        public void Evaluate_WhenThunderbirdHasZeroTopSpeed_ReturnsInvalidMove()
        {
            // Arrange
            var input = CreateMovementInput(0);

            var routeFinder = Substitute.For<IRouteFinder>();

            var movementEvaluator = CreateMovementEvaluator(routeFinder);

            // Act
            var result = movementEvaluator.Evaluate(input);

            // Assert
            Assert.False(result.IsMoveValid);
            Assert.Empty(result.Route);
            Assert.Equal(0, result.SpacesTravelled);
            Assert.Equal(0, result.TopSpeed);
            Assert.Equal(0, result.ActionPointCost);
            Assert.Single(result.Messages);
            Assert.Equal("thunderbird cannot move.", result.Messages.First());
        }

        [Fact]
        public void Evaluate_WhenEventCardAppliesModifier_ReturnsValidMoveWithModifiedTopSpeed()
        {
            // Arrange
            var eventCardCode = new CardCode("event-card");

            var input = CreateMovementInput(3, new[] { eventCardCode });

            var testModifierSource = new TestMovementSpeedModifierSource(eventCardCode);

            var registry = Substitute.For<IMovementSpeedModifierSourceRegistry>();
            registry.TryGetEventCard(eventCardCode, out Arg.Any<IMovementSpeedModifierSource?>())
                .Returns(x =>
                {
                    x[1] = testModifierSource;
                    return true;
                });

            var movementEvaluator = CreateMovementEvaluator(registry);

            // Act
            var result = movementEvaluator.Evaluate(input);

            // Assert
            Assert.True(result.IsMoveValid);
            Assert.Equal(1, result.TopSpeed);
            Assert.Equal(2, result.ActionPointCost);
            Assert.Single(result.Messages);
        }

        private static MovementInput CreateMovementInput(int topSpeed)
        {
            return CreateMovementInput(topSpeed, Enumerable.Empty<CardCode>());
        }

        private static MovementInput CreateMovementInput(int topSpeed, IEnumerable<CardCode> eventCards)
        {
            return new MovementInput(
                Thunderbird: new ThunderbirdContribution(new("thunderbird"), MovementDomain.Earth, topSpeed),
                Topography: new Topography([]),
                Start: new LocationCode("A"),
                Destination: new LocationCode("B"),
                EventCards: eventCards.ToList()
            );
        }

        private static MovementEvaluator CreateMovementEvaluator(IRouteFinder routeFinder)
        {
            var registry = Substitute.For<IMovementSpeedModifierSourceRegistry>();
            registry.TryGetEventCard(Arg.Any<CardCode>(), out Arg.Any<IMovementSpeedModifierSource?>()).Returns(false);

            return CreateMovementEvaluator(routeFinder, registry);
        }

        private static MovementEvaluator CreateMovementEvaluator(IMovementSpeedModifierSourceRegistry registry)
        {
            var route = new[] { new LocationCode("A"), new LocationCode("B"), new LocationCode("C") };

            var routeFinder = Substitute.For<IRouteFinder>();
            routeFinder.FindShortestRoute(Arg.Any<MovementInput>()).Returns(new RouteResult
            (
                Route: route.ToList(),
                SpacesTravelled: 2
            ));

            return CreateMovementEvaluator(routeFinder, registry);
        }

        private static MovementEvaluator CreateMovementEvaluator(IRouteFinder routeFinder, IMovementSpeedModifierSourceRegistry registry)
        {
            var actionPointCalculator = new ActionPointCalculator();
            return new MovementEvaluator(routeFinder, registry, actionPointCalculator);
        }

        private class TestMovementSpeedModifierSource : IMovementSpeedModifierSource
        {
            public TestMovementSpeedModifierSource(CardCode cardCode)
            {
                EventCardCode = cardCode;
            }

            public CardCode EventCardCode { get; }

            public AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input)
            {
                return new AppliedMovementSpeedModifier(EventCardCode, 1, "Test modifier applied.");
            }
        }
    }
}
