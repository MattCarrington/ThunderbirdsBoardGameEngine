using NSubstitute;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Loaders;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Loaders
{
    public class SnapshotLoaderTests
    {
        [Fact]
        public async Task LoadAsync_ValidSnapshot_ReturnsSnapshotAsync()
        {
            // Arrange
            var snapshot = ValidSnapshot();

            var loader = CreateLoader(snapshot);

            // Act
            var result = await loader.LoadAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Same(result, snapshot);
        }

        [Fact]
        public async Task LoadAsync_WithStream_DisposesStream()
        {
            // Arrange
            var stream = Substitute.For<Stream>();
            stream.When(s => s.Dispose()).DoNotCallBase();

            var provider = Substitute.For<ISnapshotProvider>();
            provider.GetSnapshotStreamAsync().Returns(Task.FromResult(stream));

            var deserializer = Substitute.For<ISnapshotDeserializer>();
            deserializer.Deserialize(Arg.Any<Stream>()).Returns(ValidSnapshot());

            var loader = new SnapshotLoader(provider, deserializer);

            // Act
            await loader.LoadAsync();

            // Assert
            await stream.Received(1).DisposeAsync();
        }

        [Fact]
        public async Task LoadAsync_WhenSchemaVersionIsUnsupported_ThrowsInvalidOperationExceptionAsync()
        {
            // Arrange
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithSchemaVersion(999) // Unsupported version
                .WithContentVersion("1.0")
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("character-1", 1, null))
                .Build();

            var loader = CreateLoader(snapshot);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadAsync());
        }

        [Theory]
        [ClassData(typeof(WhiteSpaceStringData))]
        public async Task LoadAsync_WhenContentVersionIsMissing_ThrowsInvalidOperationException(string contentVersion)
        {
            // Arrange
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithContentVersion(contentVersion)
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("character-1", 1, null))
                .Build();

            var loader = CreateLoader(snapshot);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadAsync());
        }

        [Fact]
        public async Task LoadAsync_WhenSnapshotContainsNoDisasters_ThrowsInvalidOperationExceptionAsync()
        {
            // Arrange
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .Build();

            var loader = CreateLoader(snapshot);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadAsync());
        }

        [Fact]
        public async Task LoadAsync_WhenSnapshotContainsNoCharacters_ThrowsInvalidOperationExceptionAsync()
        {
            // Arrange
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("character-1", 1, null))
                .Build();

            var loader = CreateLoader(snapshot);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadAsync());
        }

        [Fact]
        public async Task LoadAsync_WhenSnapshotContentVersionNull_ThrowsInvalidOperationExceptionAsync()
        {
            // Arrange
            var snapshot = new ReferenceDataSnapshot(
                SchemaVersion: SnapshotVersions.SchemaVersion,
                ContentVersion: null,
                GeneratedAt: DateTimeOffset.UtcNow,
                GeneratorVersion: SnapshotVersions.GeneratorVersion,
                DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                LocationDefinitions: new List<ReferenceLocationDefinition>(),
                CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>());

            var loader = CreateLoader(snapshot);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadAsync());
        }

        private static ReferenceDataSnapshot ValidSnapshot()
        {
            return new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("character-1", 1, null))
                .Build();
        }

        private static SnapshotLoader CreateLoader(ReferenceDataSnapshot snapshot)
        {
            var deserializer = Substitute.For<ISnapshotDeserializer>();
            deserializer.Deserialize(Arg.Any<Stream>()).Returns(snapshot);

            var provider = Substitute.For<ISnapshotProvider>();
            provider.GetSnapshotStreamAsync().Returns(Task.FromResult<Stream>(new MemoryStream()));

            return new SnapshotLoader(provider, deserializer);
        }
    }
}
