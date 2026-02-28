using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Output;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;

var source = new ExcelReferenceDataSource("DisasterCards.xlsx");

var context = source.Load();

var validator = new SnapshotValidator();
validator.Validate(context);

var builder = new SnapshotBuilder();
var snapshot = builder.Build(context);

JsonSnapshotWriter.Write(snapshot);

Console.WriteLine("Snapshot generated successfully.");