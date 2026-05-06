using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Deserializers;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Loaders;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Providers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.ComponentTests
{
    public class SnapshotLoadingTests
    {
        [Fact]
        public async Task CanLoadSnapshotFromEmbeddedResource()
        {
            // Arrange - real implementations, real embedded resource
            var provider = new EmbeddedResourceSnapshotProvider();
            var deserializer = new JsonSnapshotDeserializer();
            var loader = new SnapshotLoader(provider, deserializer);

            // Act
            var snapshot = await loader.LoadAsync();

            // Assert
            Assert.NotNull(snapshot);
            Assert.Equal(1, snapshot.SchemaVersion);
            Assert.Equal(34, snapshot.DisasterDefinitions.Count);
        }
    }
}