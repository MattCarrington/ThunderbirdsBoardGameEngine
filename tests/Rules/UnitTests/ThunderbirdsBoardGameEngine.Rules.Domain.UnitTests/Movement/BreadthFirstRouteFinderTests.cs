using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Movement
{
    public class BreadthFirstRouteFinderTests
    {
        private readonly ReferenceLocationDefinition _locationA = new(code: new LocationCode("A"), displayName: "Location A", domain: MovementDomain.Earth);
        private readonly ReferenceLocationDefinition _locationB = new(code: new LocationCode("B"), displayName: "Location B", domain: MovementDomain.Earth);
        private readonly ReferenceLocationDefinition _locationC = new(code: new LocationCode("C"), displayName: "Location C", domain: MovementDomain.Earth);
        private readonly ReferenceLocationDefinition _locationD = new(code: new LocationCode("D"), displayName: "Location D", domain: MovementDomain.Earth);
        private readonly ReferenceLocationDefinition _locationE = new(code: new LocationCode("E"), displayName: "Location E", domain: MovementDomain.Earth);
        private readonly ReferenceLocationDefinition _locationF = new(code: new LocationCode("F"), displayName: "Location F", domain: MovementDomain.Earth);
        private readonly ReferenceLocationDefinition _locationG = new(code: new LocationCode("G"), displayName: "Location G", domain: MovementDomain.Earth);
        private readonly ReferenceLocationDefinition _locationH = new(code: new LocationCode("H"), displayName: "Location H", domain: MovementDomain.Earth);
        private readonly ReferenceLocationDefinition _locationUnlinked = new(code: new LocationCode("unlinked"), displayName: "Unlinked", domain: MovementDomain.Earth);
        private readonly ReferenceLocationDefinition _locationSpace = new(code: new LocationCode("Space"), displayName: "Space", domain: MovementDomain.Space);

        private readonly LocationCode _invalidLocationCode = new("invalid-location-code");

        private readonly ThunderbirdContribution _thunderbird = new(new ThunderbirdCode("thunderbird-1"), MovementDomain.Earth);

        [Fact]
        public void FindShortestRoute_WhenDestinationIsSameAsStart_ReturnsRouteWithSingleLocation()
        {
            // Arrange
            var movementRequest = new MovementRequest(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Code,
                _locationA.Code);

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
            var movementRequest = new MovementRequest(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Code,
                _locationB.Code);

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
            var movementRequest = new MovementRequest(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Code,
                _locationE.Code);

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
            var movementRequest = new MovementRequest(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Code,
                _locationC.Code);

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
            var movementRequest = new MovementRequest(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Code,
                _locationSpace.Code);

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
            var movementRequest = new MovementRequest(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _invalidLocationCode,
                _locationB.Code);

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
            var movementRequest = new MovementRequest(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Code,
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
            var movementRequest = new MovementRequest(
                _thunderbird,
                new Topography(CreateLocations(), CreateEdges()),
                _locationA.Code,
                _locationUnlinked.Code);

            var routeFinder = new BreadthFirstRouteFinder();

            // Act
            var result = routeFinder.FindShortestRoute(movementRequest);

            // Assert
            Assert.Null(result);
        }

        private IReadOnlyCollection<ReferenceLocationDefinition> CreateLocations()
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
                new(edge1: _locationA.Code, edge2: _locationB.Code, edgeType: MovementDomain.Earth),
                new(edge1: _locationB.Code, edge2: _locationC.Code, edgeType: MovementDomain.Earth),
                new(edge1: _locationA.Code, edge2: _locationC.Code, edgeType: MovementDomain.Earth),
                new(edge1: _locationD.Code, edge2: _locationB.Code, edgeType: MovementDomain.Earth),
                new(edge1: _locationE.Code, edge2: _locationD.Code, edgeType: MovementDomain.Earth),
                new(edge1: _locationA.Code, edge2: _locationF.Code, edgeType: MovementDomain.Earth),
                new(edge1: _locationF.Code, edge2: _locationG.Code, edgeType: MovementDomain.Earth),
                new(edge1: _locationG.Code, edge2: _locationH.Code, edgeType: MovementDomain.Earth),
                new(edge1: _locationH.Code, edge2: _locationC.Code, edgeType: MovementDomain.Earth),
                new(edge1: _locationA.Code, edge2: _locationSpace.Code, edgeType: MovementDomain.Space),
            ];
        }
    }
}
