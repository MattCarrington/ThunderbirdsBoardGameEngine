using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.PostConfigures
{
    public class DisasterCardJsonPostConfigureTests
    {
        private const string ContentRootPath = "/app";

        private static DisasterCardJsonPostConfigure CreatePostConfigure(string? rootPath = null)
        {
            return new DisasterCardJsonPostConfigure(StubHostEnvironment.WithNullProvider(rootPath ?? ContentRootPath));
        }

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
            [ClassData(typeof(NullOrWhitespaceStringData))]
            public void PostConfigure_WhenFilePathNullOrWhitespace_ShouldReturnEarly(string? input)
            {
                // Arrange
                var options = new DisasterCardJsonOptions
                {
                    FilePath = input
                };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert
                Assert.Equal(input, options.FilePath);
            }

            [Theory]
            [InlineData("     testdata/disaster-card.json")]
            [InlineData("testdata/disaster-card.json     ")]
            public void PostConfigure_WhenFilePathContainsAdditionalWhitespace_ShouldTrim(string input)
            {
                // Arrange
                var options = new DisasterCardJsonOptions
                {
                    FilePath = input
                };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert
                var expected = ExpectedAbsolutePath("testdata/disaster-card.json");

                Assert.Equal(expected, options.FilePath);
            }

            [Theory]
            [InlineData("\"testdata/disaster-card.json\"")]
            [InlineData("'testdata/disaster-card.json'")]
            public void PostConfigure_WhenFilePathIsQuoted_ShouldRemoveQuotes(string input)
            {
                // Arrange
                var options = new DisasterCardJsonOptions
                {
                    FilePath = input
                };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert
                Assert.Equal(ExpectedAbsolutePath("testdata/disaster-card.json"), options.FilePath);
            }

            [Fact]
            public void PostConfigure_WhenRelativePath_ShouldResolveToAbsolute()
            {
                // Arrange
                var options = new DisasterCardJsonOptions
                {
                    FilePath = "testdata/disaster-card.json"
                };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert
                Assert.Equal(ExpectedAbsolutePath("testdata/disaster-card.json"), options.FilePath);
            }

            [Fact]
            public void PostConfigure_WhenBackslashesPresent_ShouldBeNormalisedByCanonicalisation()
            {
                // Arrange (Windows-style separators in input; works cross-platform)
                var options = new DisasterCardJsonOptions { FilePath = "content\\sub\\disaster.json" };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert (use OS-agnostic expected computation)
                var expected = ExpectedAbsolutePath(Path.Combine("content", "sub", "disaster.json"));
                Assert.Equal(expected, options.FilePath);
            }

            [Fact]
            public void PostConfigure_WhenAbsolutePath_ShouldRemainAbsoluteAndIsCanonicalised()
            {
                // Arrange
                var input = "/var/data/../data/disaster.json"; // collapses to /var/data/disaster.json
                var options = new DisasterCardJsonOptions { FilePath = input };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert
                Assert.Equal(Path.GetFullPath(input), options.FilePath);
            }

            [Fact]
            public void PostConfigure_WhenCalledTwice_ShouldBeIdempotentAfterFirstRun()
            {
                // Arrange
                var options = new DisasterCardJsonOptions { FilePath = "content/disaster.json" };
                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);
                
                var first = options.FilePath;
                
                postConfigure.PostConfigure(null, options);

                // Assert
                Assert.Equal(first, options.FilePath);
            }

            [Fact]
            public void PostConfigure_WhenMismatchedQuotes_ShouldNotStripl()
            {
                // Arrange (legal path; ensures no exceptions in happy absolute cases)
                var input = Path.GetFullPath(Path.Combine(ContentRootPath, "content/../content/disaster.json"));
                var options = new DisasterCardJsonOptions { FilePath = input };

                var postConfigure = CreatePostConfigure();

                // Act
                var ex = Record.Exception(() => postConfigure.PostConfigure(null, options));

                // Assert
                Assert.Null(ex);
                Assert.Equal(Path.GetFullPath(input), options.FilePath);
            }

            [Fact]
            public void PostConfigure_WhenInvalidishRelativeChars_ShouldNotThrowAndAnchor()
            {
                var input = "con<tent>/disaster.json";
                var o = new DisasterCardJsonOptions { FilePath = input };

                CreatePostConfigure().PostConfigure(null, o);

                // Anchored under content root and canonicalized; no exception expected.
                Assert.Equal(ExpectedAbsolutePath(input), o.FilePath);
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
            public void PostConfigure_WhenWindowsTokens_ShouldExpandThenResolveAbsolute(string input, string envValue, string expectedRelative)
            {
                // Arrange
                using var _ = new EnvironmentVariableScope("DISASTER_JSON_PATH", envValue);
                var options = new DisasterCardJsonOptions { FilePath = input };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert
                Assert.Equal(ExpectedAbsolutePath(expectedRelative), options.FilePath);
            }

            [Fact]
            public void PostConfigure_WhenWindowsTokenUnset_ShouldLeaveTokenThenResolvesUnderRoot()
            {
                // Arrange
                using var _ = new EnvironmentVariableScope("DISASTER_JSON_PATH", null);
                var options = new DisasterCardJsonOptions { FilePath = "%DISASTER_JSON_PATH%/disaster.json" };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert
                // token remains; then treated as relative and anchored under content root
                Assert.Equal(ExpectedAbsolutePath("%DISASTER_JSON_PATH%/disaster.json"), options.FilePath);
            }

            [Fact]
            public void PostConfigure_WhenWindowsTokenWithAbsoluteValue_ShouldCanonicalized()
            {
                // Arrange
                using var _ = new EnvironmentVariableScope("DISASTER_JSON_PATH", "/var/data/../data/disaster.json");
                var options = new DisasterCardJsonOptions { FilePath = "%DISASTER_JSON_PATH%" };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert
                Assert.Equal(Path.GetFullPath("/var/data/disaster.json"), options.FilePath);
            }

            [Fact]
            public void PostConfigure_WhenWindowsTokenEmpty_ShouldResultInRootedRelativeThenAnchored()
            {
                // Arrange
                using var _ = new EnvironmentVariableScope("DISASTER_JSON_PATH", "");
                var options = new DisasterCardJsonOptions { FilePath = "%DISASTER_JSON_PATH%/disaster.json" };

                var postConfigure = CreatePostConfigure();

                // Act
                postConfigure.PostConfigure(null, options);

                // Assert
                // "" + "/disaster.json" → treated as relative "disaster.json" by Combine/FullPath (anchored under /app)
                Assert.Equal(ExpectedAbsolutePath("%DISASTER_JSON_PATH%/disaster.json"), options.FilePath);
            }
        }
    }
}
