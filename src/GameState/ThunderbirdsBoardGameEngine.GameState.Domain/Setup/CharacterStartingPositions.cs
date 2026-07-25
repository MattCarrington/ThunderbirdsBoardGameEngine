using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.Setup
{
    public sealed class CharacterStartingPositions
    {
        public static IDictionary<CharacterCode, ThunderbirdCode> GetStartingPositions()
        {
            return new Dictionary<CharacterCode, ThunderbirdCode>
            {
                { KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird1 },
                { KnownCharacterCodes.Virgil, KnownThunderbirdCodes.Thunderbird2 },
                { KnownCharacterCodes.Alan, KnownThunderbirdCodes.Thunderbird3 },
                { KnownCharacterCodes.Gordon, KnownThunderbirdCodes.Thunderbird4 },
                { KnownCharacterCodes.John, KnownThunderbirdCodes.Thunderbird5 },
                { KnownCharacterCodes.LadyPenelope, KnownThunderbirdCodes.Fab1 }
            };
        }
    }
}
