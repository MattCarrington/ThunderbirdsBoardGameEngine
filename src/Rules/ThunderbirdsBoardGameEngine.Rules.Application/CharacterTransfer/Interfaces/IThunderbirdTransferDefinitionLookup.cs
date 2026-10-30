using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.CharacterTransfer.Interfaces
{
    public interface IThunderbirdTransferDefinitionLookup
    {
        int GetThunderbirdCharacterCapacity(ThunderbirdCode thunderbirdCode);
    }
}
