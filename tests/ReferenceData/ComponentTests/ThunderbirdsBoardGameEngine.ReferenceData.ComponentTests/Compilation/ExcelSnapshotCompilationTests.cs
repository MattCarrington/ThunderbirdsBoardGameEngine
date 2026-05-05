using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Output;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;
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
            var source = new ExcelReferenceDataSource("ReferenceData.xlsx");

            var context = source.Load();

            var builder = new SnapshotBuilder();
            var snapshot = builder.Build(context);

            var validator = new SnapshotValidator();
            validator.Validate(snapshot);

            // Act
            JsonSnapshotWriter.Write(snapshot);

            // Assert
            var json = File.ReadAllText("snapshot.json");
            Assert.NotNull(json);
            Assert.NotEmpty(json);
            Assert.Contains("\"characterDefinitions\"", json);
            Assert.Contains("\"disasterDefinitions\"", json);
            Assert.Contains("\"locationDefinitions\"", json);
            Assert.Contains("\"thunderbirdDefinitions\"", json);
            Assert.Contains("\"podVehicleDefinitions\"", json);
        }
    }
}
