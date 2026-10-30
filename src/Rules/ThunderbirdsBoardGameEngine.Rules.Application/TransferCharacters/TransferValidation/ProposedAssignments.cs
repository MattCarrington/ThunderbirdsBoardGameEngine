using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public record ProposedAssignments(
        CharacterCode CharacterCode,
        ThunderbirdCode TargetThunderbirdCode);
}
