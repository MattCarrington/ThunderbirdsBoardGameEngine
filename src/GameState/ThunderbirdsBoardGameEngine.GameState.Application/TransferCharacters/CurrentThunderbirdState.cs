using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;


namespace ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters
{
    public sealed record CurrentThunderbirdState(
        ThunderbirdCode Thunderbird,
        LocationCode Location,
        IReadOnlyCollection<CharacterCode> CharactersAboard);
}
