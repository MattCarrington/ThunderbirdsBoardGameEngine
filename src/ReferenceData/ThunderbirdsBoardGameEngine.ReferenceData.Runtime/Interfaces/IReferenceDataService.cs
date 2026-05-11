using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides access to reference data for game rules and UI.
    /// </summary>
    public interface IReferenceDataService
    {
        /// <summary>
        /// 
        /// </summary>
        ReferenceDataSnapshot Snapshot { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        ReferenceDisasterDefinition? GetDisaster(CardCode code);
    }
}