using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators
{
    /// <summary>
    /// Orchestrates snapshot validation by delegating to specialized validators.
    /// </summary>
    public sealed class SnapshotValidator
    {
        private readonly IReadOnlyList<ISnapshotValidator> _validators;

        public SnapshotValidator(IEnumerable<ISnapshotValidator> validators)
        {
            _validators = validators?.ToList() ?? throw new ArgumentNullException(nameof(validators));
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
