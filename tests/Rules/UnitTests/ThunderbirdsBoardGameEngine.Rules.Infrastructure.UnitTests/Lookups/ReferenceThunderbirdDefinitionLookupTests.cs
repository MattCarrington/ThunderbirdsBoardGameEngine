using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.UnitTests.Lookups
{
    public class ReferenceThunderbirdDefinitionLookupTests
    {
        [Fact]
        public void GetThunderbirdMovementContribution_WhenValidThunderbirdCode_ReturnsThunderbirdContribution()
        {
            // Arrange
            var code = new ThunderbirdCode("test-thunderbird");

            var thunderbirdDefinition = new ReferenceThunderbirdDefinition(
                code: code,
                displayName: "Test Thunderbird",
                domain: MovementDomain.Earth,
                topSpeed: 3);

            var catalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            catalog.GetByCode(Arg.Any<ThunderbirdCode>()).Returns(thunderbirdDefinition);

            var lookup = new ReferenceThunderbirdsDefinitionLookup(catalog);

            // Act
            var result = lookup.GetThunderbirdMovementContribution(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(thunderbirdDefinition.Code, result.Key);
            Assert.Equal(thunderbirdDefinition.Domain, result.TraversalDomain);
        }

        [Fact]
        public void GetThunderbirdMovementContribution_WhenThunderbirdCodeNotFound_ThrowsReferenceDataNotFoundException()
        {
            // Arrange
            var code = new ThunderbirdCode("non-existent-thunderbird");

            var catalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            catalog.GetByCode(Arg.Any<ThunderbirdCode>()).Throws(new KeyNotFoundException());

            var lookup = new ReferenceThunderbirdsDefinitionLookup(catalog);

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataNotFoundException>(() => lookup.GetThunderbirdMovementContribution(code));
            Assert.Equal("Thunderbird", ex.ResourceType);
            Assert.Equal(code.ToString(), ex.Code);
        }
    }
}
