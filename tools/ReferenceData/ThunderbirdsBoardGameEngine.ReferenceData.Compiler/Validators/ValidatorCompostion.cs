using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators
{
    public static class ValidatorCompostion
    {
        public static IEnumerable<ISnapshotValidator> CreateValidators()
        {
            return
            [
                new EntityUniquenessValidator(),
                new MapEdgeValidator(),
                new CardUniquenessValidator(),
                new DisasterBonusLocationOverrideValidator()
            ];
        }
    }
}
