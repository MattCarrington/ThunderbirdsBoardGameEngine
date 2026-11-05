using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.Interfaces
{
    public interface IThunderbirdTransferDefinitionLookup
    {
        int GetThunderbirdCharacterCapacity(ThunderbirdCode thunderbirdCode);
    }
}
