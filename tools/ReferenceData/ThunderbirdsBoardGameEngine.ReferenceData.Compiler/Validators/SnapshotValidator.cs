using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators
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
                new DisasterBonusSystemValidator(),
                new MapEdgeValidator(),
                new CardUniquenessValidator()
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
