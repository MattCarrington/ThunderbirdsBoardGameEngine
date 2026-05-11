using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public class ReferenceDataService : IReferenceDataService
    {
        /// <summary>
        /// 
        /// </summary>
        public ReferenceDataSnapshot Snapshot { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        public ReferenceDataService(ReferenceDataSnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ReferenceDisasterDefinition? GetDisaster(CardCode code)
        {
            return Snapshot.DisasterDefinitions.FirstOrDefault(d => d.Code.Equals(code));
        }
    }
}