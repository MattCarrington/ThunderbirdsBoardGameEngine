using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Configuration;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.UnitTests.Configuration
{
    public class CatalogClientOptionsPostConfigureTests
    {
        [Theory]
        [InlineData("https://example.com/")]
        [InlineData("http://example.com/api/")]
        [InlineData("https://example.com:8080/")]
        [InlineData("https://subdomain.example.com/")]
        [InlineData("https://example.com/path/to/resource/")]
        [InlineData("https://example.com/?query=param")]
        [InlineData("https://example.com/#fragment")]
        [InlineData("https://localhost/")]
        [InlineData("https://127.0.0.1/")]
        [InlineData("ftp://example.com/")] // non-http
        public void PostConfigure_WhenBaseAddressValid_ShouldNotModify(string baseAddress)
        {
            // Arrange
            var options = new CatalogClientOptions
            {
                BaseAddress = baseAddress
            };

            var postConfigure = CreatePostConfigure();

            // Act
            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.Equal(baseAddress, options.BaseAddress);
        }

        [Theory]
        [ClassData(typeof(NullOrWhiteSpaceStringData))]
        public void PostConfigure_WhenBaseAddressNullOrWhiteSpace_ShouldReturnEarly(string emptyBaseAddress)
        {
            // Arrange
            var options = new CatalogClientOptions
            {
                BaseAddress = emptyBaseAddress
            };

            var postConfigure = CreatePostConfigure();

            // Act
            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.Equal(emptyBaseAddress, options.BaseAddress);
        }

        [Theory]
        [InlineData("         https://example.com/")]
        [InlineData("https://example.com/         ")]
        public void PostConfigure_WhenBaseAddressContainsWhitespace_ShouldTrim(string baseAddress)
        {
            // Arrange
            var options = new CatalogClientOptions
            {
                BaseAddress = baseAddress
            };

            var postConfigure = CreatePostConfigure();

            // Act
            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.Equal("https://example.com/", options.BaseAddress);
        }

        [Fact]
        public void PostConfigure_WhenTrailingSlashMissing_ShouldAddTrailingSlash()
        {
            // Arrange
            var options = new CatalogClientOptions
            {
                BaseAddress = "https://example.com/api"
            };
            var postConfigure = CreatePostConfigure();

            // Act
            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.Equal("https://example.com/api/", options.BaseAddress);
        }

        [Theory]
        [InlineData("'https://example.com/api/'")]
        [InlineData("\"https://example.com/api/\"")]
        public void PostConfigure_WhenBaseAddressQuoted_ShouldRemoveQuotes(string baseAddress)
        {
            // Arrange
            var options = new CatalogClientOptions
            {
                BaseAddress = baseAddress
            };

            var postConfigure = CreatePostConfigure();

            // Act
            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.Equal("https://example.com/api/", options.BaseAddress);
        }

        [Fact]
        public void PostConfigure_WhenCalledTwice_ShouldBeIdempotentAfterFirstRun()
        { 
            // Arrange
            var options = new CatalogClientOptions
            {
                BaseAddress = "   'https://example.com/api'   "
            };

            var postConfigure = CreatePostConfigure();

            // Act
            postConfigure.PostConfigure(null, options);

            var first = options.BaseAddress;

            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.Equal(first, options.BaseAddress);
        }

        [Theory]
        [InlineData("/api")]
        [InlineData("api/v1")]
        [InlineData("http:/example.com")] // malformed
        public void PostConfigure_RelativeOrNonHttp_OnlyTrimDequote_NoSlashAppend(string input)
        {
            // Arrange
            var options = new CatalogClientOptions
            {
                BaseAddress = input
            };

            var postConfigure = CreatePostConfigure();

            // Act
            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.Equal(input, options.BaseAddress);
        }


        private static CatalogClientOptionsPostConfigure CreatePostConfigure()
        {
            return new CatalogClientOptionsPostConfigure();
        }
    }
}
