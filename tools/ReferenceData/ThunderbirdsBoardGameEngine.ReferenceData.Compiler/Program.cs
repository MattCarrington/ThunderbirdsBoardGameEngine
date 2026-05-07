using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Writers;

var compiler = new ReferenceDataCompiler(
    new ExcelReferenceDataSource("ReferenceData.xlsx"),
    new SnapshotBuilder(new SystemClock()),
    new SnapshotValidator(),
    new JsonSnapshotWriter());

compiler.Compile();

Console.WriteLine("Snapshot generated successfully.");

Console.WriteLine("Snapshot generated successfully.");