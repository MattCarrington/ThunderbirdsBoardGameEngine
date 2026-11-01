using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain
{
    public record CharacterAssignment(CharacterCode CharacterCode, ThunderbirdCode TargetThunderbirdCode);
}
