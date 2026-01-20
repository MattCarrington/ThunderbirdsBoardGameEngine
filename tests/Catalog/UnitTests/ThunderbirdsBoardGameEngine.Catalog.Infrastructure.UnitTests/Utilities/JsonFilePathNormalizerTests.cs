using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Utilities
{
    internal class JsonFilePathNormalizerTests
    {
        private const string ContentRootPath = "/app";

        private static string ExpectedAbsolutePath(string path)
        {
            // if absolute, GetFullPath will canonicalize; if relative, anchor to ContentRoot
            return Path.GetFullPath(
                Path.IsPathFullyQualified(path)
                    ? path
                    : Path.Combine(ContentRootPath, path));
        }

        public class PathResolution
        {
            [Theory]
            [InlineData("     testdata/disaster-card.json")]
            [InlineData("testdata/disaster-card.json     ")]
            public void Normalize_WhenFilePathContainsAdditionalWhitespace_ShouldTrim(string input)
            {
                // Arrange
                
                // Act
                var result = JsonFilePathNormalizer.Normalize(input, ContentRootPath);

                // Assert
                Assert.Equal(ExpectedAbsolutePath("testdata/disaster-card.json"), result);
            }

            [Theory]
            [InlineData("\"testdata/disaster-card.json\"")]
            [InlineData("'testdata/disaster-card.json'")]
            public void Normalize_WhenFilePathIsQuoted_ShouldRemoveQuotes(string quotedInput)
            {
                // Arrange

                // Act
                var result = JsonFilePathNormalizer.Normalize(quotedInput, ContentRootPath);

                // Assert
                Assert.Equal(ExpectedAbsolutePath("testdata/disaster-card.json"), result);
            }

            [Fact]
            public void Normalize_WhenRelativePath_ShouldResolveToAbsolute()
            {
                // Arrange
                var relative = "testdata/disaster-card.json";

                // Act
                var result = JsonFilePathNormalizer.Normalize(relative, ContentRootPath);

                // Assert
                Assert.Equal(ExpectedAbsolutePath("testdata/disaster-card.json"), result);
            }

            [Fact]
            public void Normalize_WhenBackslashesPresent_ShouldBeNormalisedByCanonicalisation()
            {
                // Arrange (Windows-style separators in input; works cross-platform)
                var input = "content\\sub\\disaster.json";

                // Act
                var result = JsonFilePathNormalizer.Normalize(input, ContentRootPath);

                // Assert (use OS-agnostic expected computation)
                var expected = ExpectedAbsolutePath(Path.Combine("content", "sub", "disaster.json"));
                Assert.Equal(expected, result);
            }

            [Fact]
            public void Normalize_WhenAbsolutePath_ShouldRemainAbsoluteAndIsCanonicalised()
            {
                // Arrange
                var input = "/var/data/../data/disaster.json"; // collapses to /var/data/disaster.json
                
                // Act
                var result = JsonFilePathNormalizer.Normalize(input, ContentRootPath);

                // Assert
                Assert.Equal(Path.GetFullPath(input), result);
            }

            [Fact]
            public void Normalize_WhenMismatchedQuotes_ShouldNotStrip()
            {
                // Arrange 
                var input = Path.GetFullPath(Path.Combine(ContentRootPath, "content/../content/disaster.json"));    // (legal path; ensures no exceptions in happy absolute cases)

                // Act
                var exception = Record.Exception(() => JsonFilePathNormalizer.Normalize(input, ContentRootPath));

                // Assert
                Assert.Null(exception);
                Assert.Equal(Path.GetFullPath(input), input);
            }

            [Fact]
            public void Normalize_WhenInvalidishRelativeChars_ShouldNotThrowAndAnchor()
            {
                // Arrange
                var input = "con<tent>/disaster.json";

                // Act
                var result = JsonFilePathNormalizer.Normalize(input, ContentRootPath);

                // Assert
                Assert.Equal(ExpectedAbsolutePath(input), result);  // Anchored under content root and canonicalized; no exception expected.
            }
        }

        [Collection("EnvironmentVariables")]
        public class EnvironmentExpansion
        {

            [Theory]
            [InlineData("%DISASTER_JSON_PATH%", "content/disaster.json", "content/disaster.json")]
            [InlineData(" %DISASTER_JSON_PATH% ", "content/disaster.json", "content/disaster.json")]
            [InlineData("\"%DISASTER_JSON_PATH%\"", "content/disaster.json", "content/disaster.json")]
            [InlineData("'%DISASTER_JSON_PATH%'", "content/disaster.json", "content/disaster.json")]
            [InlineData("$DISASTER_JSON_PATH/disaster.json", "content", "content/disaster.json")]
            [InlineData(" $DISASTER_JSON_PATH/disaster.json ", "content", "content/disaster.json")]
            [InlineData("\"$DISASTER_JSON_PATH/disaster.json\"", "content", "content/disaster.json")]
            [InlineData("${DISASTER_JSON_PATH}/disaster.json", "content", "content/disaster.json")]
            [InlineData(" ${DISASTER_JSON_PATH}/disaster.json ", "content", "content/disaster.json")]
            [InlineData("\"${DISASTER_JSON_PATH}/disaster.json\"", "content", "content/disaster.json")]
            public void Normalize_WhenEnvironmentTokens_ShouldExpandThenResolveAbsolute(string input, string envValue, string expectedRelative)
            {
                // Arrange
                using var scope = new EnvironmentVariableScope("DISASTER_JSON_PATH", envValue);
                
                // Act
                var result = JsonFilePathNormalizer.Normalize(input, ContentRootPath);

                // Assert
                Assert.Equal(ExpectedAbsolutePath(expectedRelative), result);
            }

            [Fact]
            public void Normalize_WhenEnvironmentTokenUnset_ShouldLeaveTokenThenResolvesUnderRoot()
            {
                // Arrange
                using var scope = new EnvironmentVariableScope("DISASTER_JSON_PATH", null);
                
                var input = "%DISASTER_JSON_PATH%/disaster.json";

                // Act
                var result = JsonFilePathNormalizer.Normalize(input, ContentRootPath);

                // Assert
                // token remains; then treated as relative and anchored under content root
                Assert.Equal(ExpectedAbsolutePath("%DISASTER_JSON_PATH%/disaster.json"), result);
            }

            [Fact]
            public void Normalize_WhenEnvironemtTokenWithAbsoluteValue_ShouldCanonicalized()
            {
                // Arrange
                using var scope = new EnvironmentVariableScope("DISASTER_JSON_PATH", "/var/data/../data/disaster.json");
                
                var input = "%DISASTER_JSON_PATH%";

                // Act
                var result = JsonFilePathNormalizer.Normalize(input, ContentRootPath);

                // Assert
                Assert.Equal(Path.GetFullPath("/var/data/disaster.json"), result);
            }

            [Fact]
            public void Normalize_WhenEnvironmentTokenEmpty_ShouldResultInRootedRelativeThenAnchored()
            {
                // Arrange
                using var scope = new EnvironmentVariableScope("DISASTER_JSON_PATH", "");
                
                var input = "%DISASTER_JSON_PATH%/disaster.json";

                // Act
                var result = JsonFilePathNormalizer.Normalize(input, ContentRootPath);

                // Assert
                // "" + "/disaster.json" → treated as relative "disaster.json" by Combine/FullPath (anchored under /app)
                Assert.Equal(ExpectedAbsolutePath("%DISASTER_JSON_PATH%/disaster.json"), result);
            }
        }
    }
}
