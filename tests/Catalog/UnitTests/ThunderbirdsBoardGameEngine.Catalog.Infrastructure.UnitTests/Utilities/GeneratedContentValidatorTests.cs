using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Utilities
{
    public class GeneratedContentValidatorTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Validate_WhenItemCountInvalid_ThrowsInvalidDataException(int itemCount)
        {
            // Arrange
            var validator = CreateValidator();

            var payload = new GeneratedManifestPayloadBuilder()
                .WithItemCount(itemCount)
                .Build();

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => validator.Validate<GeneratedCatalogManifest>(payload));
        }

        [Fact]
        public void Validate_WhenItemCountDoesNotMatchData_ThrowsInvalidDataException()
        {
            // Arrange
            var validator = CreateValidator();

            var payload = new GeneratedManifestPayloadBuilder()
                .WithItemCount(3) // Intentionally incorrect
                .Build();

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => validator.Validate<GeneratedCatalogManifest>(payload));
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Validate_WhenAlgorithmNullOrWhiteSpace_ThrowsInvalidDataException(string algorithm)
        {
            // Arrange
            var validator = CreateValidator();

            var payload = new GeneratedManifestPayloadBuilder()
                .WithChecksumAlgorithm(algorithm)
                .Build();

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => validator.Validate<GeneratedCatalogManifest>(payload));
        }

        [Fact]
        public void Validate_WhenAlgorithmNotSupported_ThrowsNotSupportedException()
        {
            // Arrange
            var validator = CreateValidator();

            var payload = new GeneratedManifestPayloadBuilder()
                .WithChecksumAlgorithm("md5")
                .Build();

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => validator.Validate<GeneratedCatalogManifest>(payload));
        }

        [Theory]
        [ClassData(typeof(WhiteSpaceStringData))]        
        public void Validate_WhenChecksumValueMissing_ThrowsInvalidDataException(string checksum)
        {
            // Arrange
            var validator = CreateValidator();

            var payload = new GeneratedManifestPayloadBuilder()
                .WithChecksumOverride(checksum)
                .Build();

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => validator.Validate<GeneratedCatalogManifest>(payload));
        }

        [Fact]
        public void Validate_WhenChecksumValueDoesNotMatchExpected_ThrowsInvalidDataException()
        {
            // Arrange
            var validator = CreateValidator();

            var payload = new GeneratedManifestPayloadBuilder()
                .WithChecksumOverride("invalidchecksum")
                .Build();

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => validator.Validate<GeneratedCatalogManifest>(payload));
        }

        private static GeneratedContentValidator CreateValidator()
        {
            return new GeneratedContentValidator();
        }
    }
}
