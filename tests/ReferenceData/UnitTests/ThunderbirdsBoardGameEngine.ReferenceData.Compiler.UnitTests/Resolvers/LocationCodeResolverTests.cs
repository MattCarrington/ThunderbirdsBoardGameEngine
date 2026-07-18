using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Resolvers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Resolvers
{
    public class LocationCodeResolverTests
    {
        [Fact]
        public void Resolve_WhenLocationNameExists_ShouldReturnCorrectLocationCode()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var result = resolver.Resolve("Location A");

            // Assert
            Assert.Equal(new LocationCode("LOC_A"), result);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Resolve_WhenLocationNameIsNullOrWhitespace_ShouldThrowArgumentException(string? locationName)
        {
            // Arrange
            var resolver = CreateResolver();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => resolver.Resolve(locationName));
            Assert.Equal("Location name cannot be null or whitespace. (Parameter 'locationName')", exception.Message);
        }

        [Fact]
        public void Resolve_WhenLocationNameDoesNotExist_ShouldThrowReferenceDataCompilationException()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => resolver.Resolve("Nonexistent Location"));
            Assert.Equal("Location with name 'Nonexistent Location' not found.", exception.Message);
        }

        [Fact]
        public void Resolve_WhenLocationNameExistsInDifferentCase_ShouldReturnCorrectLocationCode()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var result = resolver.Resolve("location a");

            // Assert
            Assert.Equal(new LocationCode("LOC_A"), result);
        }

        [Theory]
        [InlineData("    Location A")]
        [InlineData("Location A    ")]
        [InlineData("  Location A  ")]
        public void Resolve_WhenLocationNameExistsWithLeadingOrTrailingWhitespace_ShouldReturnCorrectLocationCode(string locationName)
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var result = resolver.Resolve(locationName);

            // Assert
            Assert.Equal(new LocationCode("LOC_A"), result);
        }

        [Fact]
        public void Constructor_WhenLocationNamesAreNotUnique_ShouldThrowReferenceDataCompilationException()
        {
            // Arrange
            var locations = new List<ReferenceLocationDefinition>
            {
                new(displayName: "Location A", code: new LocationCode("LOC_A"), domain: MovementDomain.Earth),
                new(displayName: "Location A", code: new LocationCode("LOC_B"), domain: MovementDomain.Earth)
            };

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => new LocationCodeResolver(locations));
            Assert.Equal(
                "Location names must be unique for relationship resolution. Ambiguous names: Location A",
                exception.Message);
        }

        private static LocationCodeResolver CreateResolver()
        {
            var locations = new List<ReferenceLocationDefinition>
            {
                new(displayName: "Location A", code: new LocationCode("LOC_A"), domain: MovementDomain.Earth),
                new(displayName: "Location B", code: new LocationCode("LOC_B"), domain: MovementDomain.Earth)
            };

            return new LocationCodeResolver(locations);
        }
    }
}
