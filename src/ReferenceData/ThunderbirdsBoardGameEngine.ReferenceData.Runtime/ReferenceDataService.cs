using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime
{
    public class ReferenceDataService : IReferenceDataService
    {
        public ReferenceDataSnapshot Snapshot { get; }

        public ReferenceDataService(ReferenceDataSnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public ReferenceDisasterDefinition? GetDisaster(CardCode code)
        {
            return Snapshot.DisasterDefinitions.FirstOrDefault(d => d.Code.Equals(code));
        }
    }
}