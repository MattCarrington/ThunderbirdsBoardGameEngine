using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public sealed record ProposedCharacterAssignment(
        CharacterCode CharacterCode,
        ThunderbirdCode TargetThunderbirdCode);
}
