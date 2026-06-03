using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Movement
{
    public class BreadthFirstRouteFinderTests
    {
        private readonly LocationContribution _locationA = new(Key: new LocationCode("A"), Location: MovementDomain.Earth);
        private readonly LocationContribution _locationB = new(Key: new LocationCode("B"), Location: MovementDomain.Earth);
        private readonly LocationContribution _locationC = new(Key: new LocationCode("C"), Location: MovementDomain.Earth);
        private readonly LocationContribution _locationD = new(Key: new LocationCode("D"), Location: MovementDomain.Earth);
        private readonly LocationContribution _locationE = new(Key: new LocationCode("E"), Location: MovementDomain.Earth);
        private readonly LocationContribution _locationF = new(Key: new LocationCode("F"), Location: MovementDomain.Earth);
        private readonly LocationContribution _locationG = new(Key: new LocationCode("G"), Location: MovementDomain.Earth);
        private readonly LocationContribution _locationH = new(Key: new LocationCode("H"), Location: MovementDomain.Earth);
        private readonly LocationContribution _locationUnlinked = new(Key: new LocationCode("unlinked"), Location: MovementDomain.Earth);
        private readonly LocationContribution _locationSpace = new(Key: new LocationCode("Space"), Location: MovementDomain.Space);

        private readonly LocationCode _invalidLocationCode = new("invalid-location-code");

        private readonly ThunderbirdContribution _thunderbird = new(new ThunderbirdCode("thunderbird-1"), MovementDomain.Earth, 1);

        [Fact]
        public void FindShortestRoute_WhenDestinationIsSameAsStart_ReturnsRouteWithSingleLocation()
        {
            // Arrange
            var movementRequest = new MovementInput(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Key,
                _locationA.Key);

            var routeFinder = new BreadthFirstRouteFinder();

            // Act
            var result = routeFinder.FindShortestRoute(movementRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.SpacesTravelled);
            Assert.Single(result.Route);
        }

        [Fact]
        public void FindShortestRoute_WhenDestinationIsOneMoveAway_ReturnsRouteWithTwoLocations()
        {
            // Arrange
            var movementRequest = new MovementInput(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Key,
                _locationB.Key);

            var routeFinder = new BreadthFirstRouteFinder();

            // Act
            var result = routeFinder.FindShortestRoute(movementRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.SpacesTravelled);
            Assert.Equal(2, result.Route.Count);
        }

        [Fact]
        public void FindShortestRoute_DestinationIsMultipleStepsAway_ReturnsRoute()
        {
            // Arrange
            var movementRequest = new MovementInput(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Key,
                _locationE.Key);

            var routeFinder = new BreadthFirstRouteFinder();

            // Act
            var result = routeFinder.FindShortestRoute(movementRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.SpacesTravelled);
            Assert.Equal(4, result.Route.Count);
        }

        [Fact]
        public void FindShortestRoute_MultipleRoutesExist_ReturnsShortestRoute()
        {
            // Arrange
            var movementRequest = new MovementInput(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Key,
                _locationC.Key);

            var routeFinder = new BreadthFirstRouteFinder();

            // Act
            var result = routeFinder.FindShortestRoute(movementRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.SpacesTravelled);
            Assert.Equal(2, result.Route.Count);
        }

        [Fact]
        public void FindShortestRoute_RouteTraversesCrossDomain_ReturnsNull()
        {
            // Arrange
            var movementRequest = new MovementInput(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Key,
                _locationSpace.Key);

            var routeFinder = new BreadthFirstRouteFinder();

            // Act
            var result = routeFinder.FindShortestRoute(movementRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void FindShortestRoute_StartLocationDoesNotExist_ReturnsNull()
        {
            // Arrange
            var movementRequest = new MovementInput(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _invalidLocationCode,
                _locationB.Key);

            var routeFinder = new BreadthFirstRouteFinder();

            // Act
            var result = routeFinder.FindShortestRoute(movementRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void FindShortestRoute_DestinationLocationDoesNotExist_ReturnsNull()
        {
            // Arrange
            var movementRequest = new MovementInput(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Key,
                _invalidLocationCode);

            var routeFinder = new BreadthFirstRouteFinder();

            // Act
            var result = routeFinder.FindShortestRoute(movementRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void FindShortestRoute_NoLinkBetweenStartAndDestination_ReturnsNull()
        {
            // Arrange
            var movementRequest = new MovementInput(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Key,
                _locationUnlinked.Key);

            var routeFinder = new BreadthFirstRouteFinder();

            // Act
            var result = routeFinder.FindShortestRoute(movementRequest);

            // Assert
            Assert.Null(result);
        }

        private IReadOnlyCollection<LocationContribution> CreateLocations()
        {
            return
            [
                _locationA,
                _locationB,
                _locationC,
                _locationD,
                _locationE,
                _locationF,
                _locationG,
                _locationH,
                _locationSpace,
                _locationUnlinked
            ];
        }

        private IReadOnlyCollection<ReferenceMapEdgeDefinition> CreateEdges()
        {
            return
            [
                new(edge1: _locationA.Key, edge2: _locationB.Key, edgeType: MovementDomain.Earth),
                new(edge1: _locationB.Key, edge2: _locationC.Key, edgeType: MovementDomain.Earth),
                new(edge1: _locationA.Key, edge2: _locationC.Key, edgeType: MovementDomain.Earth),
                new(edge1: _locationD.Key, edge2: _locationB.Key, edgeType: MovementDomain.Earth),
                new(edge1: _locationE.Key, edge2: _locationD.Key, edgeType: MovementDomain.Earth),
                new(edge1: _locationA.Key, edge2: _locationF.Key, edgeType: MovementDomain.Earth),
                new(edge1: _locationF.Key, edge2: _locationG.Key, edgeType: MovementDomain.Earth),
                new(edge1: _locationG.Key, edge2: _locationH.Key, edgeType: MovementDomain.Earth),
                new(edge1: _locationH.Key, edge2: _locationC.Key, edgeType: MovementDomain.Earth),
                new(edge1: _locationA.Key, edge2: _locationSpace.Key, edgeType: MovementDomain.Space),
            ];
        }
    }
}
