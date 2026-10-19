using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public sealed record ThunderbirdTransferState(
        ThunderbirdCode ThunderbirdCode,
        int CharacterCapacity,
        LocationCode CurrentLocation,
        IReadOnlyCollection<CharacterCode> Occupants);
}
