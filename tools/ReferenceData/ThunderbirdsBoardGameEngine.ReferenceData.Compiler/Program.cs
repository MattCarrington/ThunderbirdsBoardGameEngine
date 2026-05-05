using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Output;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;

var source = new ExcelReferenceDataSource("ReferenceData.xlsx");

var context = source.Load();

var clock = new SystemClock();

var builder = new SnapshotBuilder(clock);
var snapshot = builder.Build(context);

var validator = new SnapshotValidator();
validator.Validate(snapshot);

JsonSnapshotWriter.Write(snapshot);

Console.WriteLine("Snapshot generated successfully.");