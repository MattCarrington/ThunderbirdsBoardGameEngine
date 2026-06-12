using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Writers;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
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

                var compiler = new ReferenceDataCompiler(
                    new ExcelReferenceDataSource("ReferenceData.xlsx"),
                    new SnapshotBuilder(new FakeClock(dateTimeOffset)),
                    new SnapshotValidator(),
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
