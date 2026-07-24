using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Movement
{
    public class EffectiveTopographyResolverTests
    {
        [Fact]
        public void Resolve_WhenMultipleModifiersApply_StacksBlockedEdgesAndMessages()
        {
            // Arrange
            var card1 = new CardCode("card-1");
            var card2 = new CardCode("card-2");

            var edge1 = CreateEdge("A", "B");
            var edge2 = CreateEdge("B", "C");

            var remainingEdge = CreateEdge("C", "D");

            var baseTopography = new Topography([edge1, edge2, remainingEdge]);

            var registry = new MovementTopologyModifierSourceRegistry(
            [
                new TestSource(card1, new BlockedMovementEdge(edge1.Edge1, edge1.Edge2), "First modifier"),
                new TestSource(card2, new BlockedMovementEdge(edge2.Edge1, edge2.Edge2), "Second modifier")
            ]);

            var resolver = new EffectiveTopographyResolver(registry);

            // Act
            var result = resolver.Resolve(baseTopography, [card1, card2]);

            // Assert
            Assert.Equal([remainingEdge], result.Value.Edges);
            Assert.Equal(["First modifier", "Second modifier"], result.Messages);
            Assert.Equal(3, baseTopography.Edges.Count);
        }

        [Fact]
        public void Resolve_WhenNoModifierApplies_PreservesBaseTopography()
        {
            // Arrange
            var edge = CreateEdge("A", "B");

            var baseTopography = new Topography([edge]);

            var resolver = new EffectiveTopographyResolver(new MovementTopologyModifierSourceRegistry([]));

            // Act
            var result = resolver.Resolve(baseTopography, [new CardCode("unrelated-card")]);

            // Assert
            Assert.Equal([edge], result.Value.Edges);
            Assert.Empty(result.Messages);
        }

        private static ReferenceMapEdgeDefinition CreateEdge(string location1, string location2)
        {
            return new ReferenceMapEdgeDefinition(
                new LocationCode(location1),
                new LocationCode(location2),
                MovementDomain.Earth);
        }

        private sealed class TestSource : IMovementTopologyModifierSource
        {
            private readonly BlockedMovementEdge _blockedEdge;
            private readonly string _message;

            public TestSource(CardCode eventCardCode, BlockedMovementEdge blockedEdge, string message)
            {
                CardCode = eventCardCode;
                _blockedEdge = blockedEdge;
                _message = message;
            }

            public CardCode CardCode { get; }

            public AppliedMovementTopologyModifier ApplyMovementModifier()
            {
                return new AppliedMovementTopologyModifier(CardCode, [_blockedEdge], _message);
            }
        }
    }
}
