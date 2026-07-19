using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators
{
    public class DisasterBonusLocationOverrideValidator : ISnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            foreach (var disaster in snapshot.DisasterDefinitions)
            {
                foreach (var bonus in disaster.Bonuses)
                {
                    if (bonus.Location is not null &&
                        bonus.Location == disaster.Location)
                    {
                        throw new ReferenceDataCompilationException(
                            $"Bonus target '{bonus.Key.Value}' on disaster " +
                            $"'{disaster.DisplayName}' explicitly specifies the " +
                            $"disaster location '{disaster.Location.Value}'. " +
                            "Leave the bonus location empty to inherit the " +
                            "disaster location.");
                    }
                }
            }
        }
    }
}
