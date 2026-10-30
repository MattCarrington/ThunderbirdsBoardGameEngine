using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public record ThunderbirdGameState(
        ThunderbirdCode ThunderbirdCode,
        LocationCode CurrentLocation,
        IReadOnlyList<CharacterCode> Occupants);
}
