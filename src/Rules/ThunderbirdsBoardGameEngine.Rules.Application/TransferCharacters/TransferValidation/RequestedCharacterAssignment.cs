using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public record RequestedCharacterAssignment(
        CharacterCode CharacterCode,
        ThunderbirdCode TargetThunderbirdCode);
}
