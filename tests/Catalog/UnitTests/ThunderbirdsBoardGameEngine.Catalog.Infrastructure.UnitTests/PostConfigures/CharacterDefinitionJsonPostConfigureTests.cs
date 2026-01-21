using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.PostConfigures
{
    public class CharacterDefinitionJsonPostConfigureTests
    {
        private const string ContentRootPath = "/app";

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void PostConfigure_WhenFilePathNullOrWhitespace_ShouldReturnEarly(string? input)
        {
            // Arrange
            var options = new CharacterJsonOptions
            {
                FilePath = input
            };

            var postConfigure = CreatePostConfigure();

            // Act
            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.Equal(input, options.FilePath);
        }

        [Fact]
        public void PostConfigure_WhenValidPath_DelegatesToNormalizer()
        {
            // Arrange
            var options = new CharacterJsonOptions
            {
                FilePath = "content/disaster.json"
            };

            var postConfigure = CreatePostConfigure();

            // Act
            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.True(Path.IsPathFullyQualified(options.FilePath));
        }

        private static CharacterDefinitionJsonPostConfigure CreatePostConfigure(string? rootPath = null)
        {
            return new CharacterDefinitionJsonPostConfigure(StubHostEnvironment.WithNullProvider(rootPath ?? ContentRootPath));
        }
    }
}
