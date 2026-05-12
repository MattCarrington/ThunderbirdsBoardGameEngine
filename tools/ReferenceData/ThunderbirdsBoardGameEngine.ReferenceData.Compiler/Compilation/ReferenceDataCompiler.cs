using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public class ReferenceDataCompiler
    {
        private readonly IReferenceDataSource _referenceDataSource;
        private readonly SnapshotBuilder _snapshotBuilder;
        private readonly SnapshotValidator _snapshotValidator;
        private readonly ISnapshotWriter _snapshotWriter;

        public ReferenceDataCompiler(
            IReferenceDataSource referenceDataSource,
            SnapshotBuilder snapshotBuilder,
            SnapshotValidator snapshotValidator,
            ISnapshotWriter snapshotWriter)
        {
            _referenceDataSource = referenceDataSource;
            _snapshotBuilder = snapshotBuilder;
            _snapshotValidator = snapshotValidator;
            _snapshotWriter = snapshotWriter;
        }

        public void Compile()
        {
            var context = _referenceDataSource.Load();
            var snapshot = _snapshotBuilder.Build(context);
            _snapshotValidator.Validate(snapshot);
            _snapshotWriter.Write(snapshot);
        }
    }
}
