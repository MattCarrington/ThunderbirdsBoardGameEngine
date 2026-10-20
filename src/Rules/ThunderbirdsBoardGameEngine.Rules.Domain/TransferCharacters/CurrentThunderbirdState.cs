using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters
{
    public sealed record CurrentThunderbirdState(
        ThunderbirdCode ThunderbirdCode,
        LocationCode CurrentLocation,
        IReadOnlyCollection<CharacterCode> Occupants);
}
