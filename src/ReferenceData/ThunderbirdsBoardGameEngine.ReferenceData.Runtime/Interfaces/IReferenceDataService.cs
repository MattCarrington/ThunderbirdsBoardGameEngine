using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides access to reference data for game rules and UI.
    /// </summary>
    public interface IReferenceDataService
    {
        ReferenceDataSnapshot Snapshot { get; }

        ReferenceDisasterDefinition? GetDisaster(CardCode code);
    }
}