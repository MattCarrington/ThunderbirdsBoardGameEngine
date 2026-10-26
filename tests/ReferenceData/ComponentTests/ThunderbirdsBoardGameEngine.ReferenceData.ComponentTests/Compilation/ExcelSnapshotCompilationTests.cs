using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Writers;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Compilation
{
    public class ExcelSnapshotCompilationTests
    {
        [Fact]
        public void EmbeddedExcelCompilesToSnapshot()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            var outputPath = Path.Combine(tempDirectory, "snapshot.json");

            try
            {
                var dateTimeOffset = new DateTimeOffset(2026, 6, 1, 12, 0, 0, TimeSpan.Zero);

                var validators = ValidatorComposition.CreateValidators();

                var compiler = new ReferenceDataCompiler(
                    new ExcelReferenceDataSource("ReferenceData.xlsx"),
                    new SnapshotBuilder(new FakeClock(dateTimeOffset)),
                    new SnapshotValidator(validators),
                    new JsonSnapshotWriter(outputPath));

                // Act
                compiler.Compile();

                // Assert
                var json = File.ReadAllText(outputPath);
                Assert.NotNull(json);
                Assert.NotEmpty(json);

                var snapshot = JsonSerializer.Deserialize<ReferenceDataSnapshot>(json, SnapshotJsonOptions.Default);
                Assert.NotNull(snapshot);

                Assert.Equal(SnapshotVersions.SchemaVersion, snapshot.SchemaVersion);
                Assert.Equal(SnapshotVersions.ContentVersion, snapshot.ContentVersion);
                Assert.Equal(dateTimeOffset, snapshot.GeneratedAt);
                Assert.Equal(SnapshotVersions.GeneratorVersion, snapshot.GeneratorVersion);

                Assert.NotEmpty(snapshot.CharacterDefinitions);
                Assert.NotEmpty(snapshot.DisasterDefinitions);
                Assert.NotEmpty(snapshot.LocationDefinitions);
                Assert.NotEmpty(snapshot.ThunderbirdDefinitions);
                Assert.NotEmpty(snapshot.PodVehicleDefinitions);
                Assert.NotEmpty(snapshot.MapEdgeDefinitions);
                Assert.NotEmpty(snapshot.FabCardDefinitions);
                Assert.NotEmpty(snapshot.EventCardDefinitions);
                Assert.All(snapshot.ThunderbirdDefinitions, definition =>
                    Assert.True(definition.CharacterCapacity > 0));

                // Include nested bonuses and polymorphic rewards in the hydration check.
                Assert.All(snapshot.DisasterDefinitions, definition =>
                {
                    Assert.NotEmpty(definition.Bonuses);
                    Assert.NotEmpty(definition.Rewards);
                });

                var reserialized = JsonSerializer.Serialize(snapshot, SnapshotJsonOptions.Create());
                using var compiledDocument = JsonDocument.Parse(json);
                using var hydratedDocument = JsonDocument.Parse(reserialized);
                Assert.True(JsonElement.DeepEquals(compiledDocument.RootElement,
                    hydratedDocument.RootElement));
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, recursive: true);
                }
            }
        }
    }
}
