using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public sealed record CurrentThunderbirdState(
        ThunderbirdCode ThunderbirdCode,
        LocationCode CurrentLocation,
        IReadOnlyCollection<CharacterCode> Occupants);
}
