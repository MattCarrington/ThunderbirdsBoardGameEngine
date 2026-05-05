using ThunderbirdsBoardGameEngine.ReferenceData.Compiler;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Output;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Compilation
{
    public class ExcelSnapshotCompilationTests
    {
        [Fact]
        public void EmbeddedExcelCompilesToSnapshot()
        {
            // Arrange
            var dateTimeOffset = new DateTimeOffset(2026, 6, 1, 12, 0, 0, TimeSpan.Zero);

            var source = new ExcelReferenceDataSource("ReferenceData.xlsx");

            var context = source.Load();

            var clock = new FakeClock(dateTimeOffset);

            var builder = new SnapshotBuilder(clock);
            var snapshot = builder.Build(context);

            var validator = new SnapshotValidator();
            validator.Validate(snapshot);

            // Act
            JsonSnapshotWriter.Write(snapshot);

            // Assert
            var json = File.ReadAllText("snapshot.json");
            Assert.NotNull(json);
            Assert.NotEmpty(json);

            Assert.Contains($"\"schemaVersion\": {SnapshotVersions.SchemaVersion}", json);
            Assert.Contains($"\"contentVersion\": \"{SnapshotVersions.ContentVersion}\"", json);
            Assert.Contains($"\"generatedAt\": \"{dateTimeOffset:O}\"", json);
            Assert.Contains($"\"generatorVersion\": \"{SnapshotVersions.GeneratorVersion}\"", json);

            Assert.Contains("\"characterDefinitions\"", json);
            Assert.Contains("\"disasterDefinitions\"", json);
            Assert.Contains("\"locationDefinitions\"", json);
            Assert.Contains("\"thunderbirdDefinitions\"", json);
            Assert.Contains("\"podVehicleDefinitions\"", json);
        }
    }
}
