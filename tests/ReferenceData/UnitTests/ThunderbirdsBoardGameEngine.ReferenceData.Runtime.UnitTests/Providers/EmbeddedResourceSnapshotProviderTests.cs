using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Providers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Providers
{
    public class EmbeddedResourceSnapshotProviderTests
    {
        [Fact]
        public async Task GetSnapshotStreamAsync_WhenResourceExists_ReturnsStream()
        {
            // Arrange
            var assembly = typeof(EmbeddedResourceSnapshotProvider).Assembly;
            var resourceName = $"{assembly.GetName().Name}.snapshot.json";

            var provider = new EmbeddedResourceSnapshotProvider(assembly, resourceName);

            // Act
            var stream = await provider.GetSnapshotStreamAsync();

            // Assert
            Assert.NotNull(stream);
        }

        [Fact]
        public async Task GetSnapshotStreamAsync_WhenResourceDoesNotExist_ThrowsInvalidOperationException()
        {
            var provider = new EmbeddedResourceSnapshotProvider(
                typeof(EmbeddedResourceSnapshotProviderTests).Assembly,
                "missing.snapshot.json");

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => provider.GetSnapshotStreamAsync());

            Assert.Equal("Embedded resource 'missing.snapshot.json' not found.", ex.Message);
        }
    }
}
