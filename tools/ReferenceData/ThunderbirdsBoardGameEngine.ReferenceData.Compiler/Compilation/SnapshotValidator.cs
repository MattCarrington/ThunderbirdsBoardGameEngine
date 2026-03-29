using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation.Validators;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    /// <summary>
    /// Orchestrates snapshot validation by delegating to specialized validators.
    /// </summary>
    public sealed class SnapshotValidator
    {
        private readonly ISnapshotValidator[] _validators;

        public SnapshotValidator()
        {
            _validators =
            [
                new EntityUniquenessValidator(),
                new LocationReferenceValidator(),
                new DisasterBonusSystemValidator()
            ];
        }

        public void Validate(ReferenceDataSnapshot snapshot)
        {
            foreach (var validator in _validators)
            {
                validator.Validate(snapshot);
            }
        }
    }
}
