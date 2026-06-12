using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
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

        private static MovementInput CreateMovementInput(int topSpeed)
        {
            return new MovementInput(
                Thunderbird: new ThunderbirdContribution(new("thunderbird"), MovementDomain.Earth, topSpeed),
                Topography: new Topography([]),
                Start: new LocationCode("A"),
                Destination: new LocationCode("B")
            );

        }

        private static MovementEvaluator CreateMovementEvaluator(IRouteFinder routeFinder)
        {
            var actionPointCalculator = new ActionPointCalculator();
            return new MovementEvaluator(routeFinder, actionPointCalculator);
        }
    }
}
