using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Output;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;

var source = new ExcelReferenceDataSource("DisasterCards.xlsx");

var context = source.Load();

var builder = new SnapshotBuilder();
var snapshot = builder.Build(context);

var validator = new SnapshotValidator();
validator.Validate(snapshot);

JsonSnapshotWriter.Write(snapshot);

Console.WriteLine("Snapshot generated successfully.");