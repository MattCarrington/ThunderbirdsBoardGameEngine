using NSubstitute;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.UnitTests.Lookups
{
    public class ReferenceThunderbirdDefinitionLookupTests
    {
        [Fact]
        public void GetThunderbirdMovementContribution_WhenValidThunderbirdCode_ReturnsThunderbirdContribution()
        {
            // Arrange
            var thunderbirdDefinition = CreateThunderbirdDefinition();

            var lookup = CreateLookupWithDefinition(thunderbirdDefinition);

            // Act
            var result = lookup.GetThunderbirdMovementContribution(thunderbirdDefinition.Code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(thunderbirdDefinition.Code, result.Key);
            Assert.Equal(thunderbirdDefinition.Domain, result.TraversalDomain);
            Assert.Equal(thunderbirdDefinition.TopSpeed, result.TopSpeed);
        }

        [Fact]
        public void GetThunderbirdMovementContribution_WhenThunderbirdCodeNotFound_ThrowsReferenceDataNotFoundException()
        {
            // Arrange
            var code = new ThunderbirdCode("non-existent-thunderbird");

            var lookup = CreateLookupWithNoDefinition();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataNotFoundException>(() => lookup.GetThunderbirdMovementContribution(code));
            Assert.Equal("Thunderbird", ex.ResourceType);
            Assert.Equal(code.ToString(), ex.Code);
        }

        [Fact]
        public void GetThunderbirdCapacity_WhenValidThunderbirdCode_ReturnsCharacterCapacity()
        {
            // Arrange
            var thunderbirdDefinition = CreateThunderbirdDefinition();

            var lookup = CreateLookupWithDefinition(thunderbirdDefinition);

            // Act
            var result = lookup.GetThunderbirdCharacterCapacity(thunderbirdDefinition.Code);

            // Assert
            Assert.Equal(thunderbirdDefinition.CharacterCapacity, result);
        }

        [Fact]
        public void GetThunderbirdCapacity_WhenThunderbirdCodeNotFound_ThrowsReferenceDataNotFound()
        {
            // Arrange
            var code = new ThunderbirdCode("non-existent-thunderbird");

            var lookup = CreateLookupWithNoDefinition();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataNotFoundException>(() => lookup.GetThunderbirdCharacterCapacity(code));
            Assert.Equal("Thunderbird", ex.ResourceType);
            Assert.Equal(code.ToString(), ex.Code);
        }

        private static ReferenceThunderbirdDefinition CreateThunderbirdDefinition()
        {
            return ReferenceDefinitionFactory.Thunderbird(
                code: new ThunderbirdCode("test-thunderbird"),
                displayName: "Test Thunderbird",
                domain: MovementDomain.Earth,
                topSpeed: 3,
                characterCapacity: 2);
        }

        private static ReferenceThunderbirdDefinitionLookup CreateLookupWithDefinition(ReferenceThunderbirdDefinition definition)
        {
            var catalog = Substitute.For<IThunderbirdDefinitionCatalog>();

            catalog.TryGetByCode(definition.Code, out Arg.Any<ReferenceThunderbirdDefinition?>())
                .Returns(call =>
                {
                    call[1] = definition;
                    return true;
                });

            return new ReferenceThunderbirdDefinitionLookup(catalog);
        }

        private static ReferenceThunderbirdDefinitionLookup CreateLookupWithNoDefinition()
        {
            var catalog = Substitute.For<IThunderbirdDefinitionCatalog>();

            catalog.TryGetByCode(Arg.Any<ThunderbirdCode>(), out Arg.Any<ReferenceThunderbirdDefinition?>())
                .Returns(call =>
                {
                    call[1] = null;
                    return false;
                });

            return new ReferenceThunderbirdDefinitionLookup(catalog);
        }

        private static ReferenceThunderbirdDefinitionLookup CreateEmptyLookup()
        {
            var catalog = Substitute.For<IThunderbirdDefinitionCatalog>();

            return new ReferenceThunderbirdDefinitionLookup(catalog);
        }
    }
}
