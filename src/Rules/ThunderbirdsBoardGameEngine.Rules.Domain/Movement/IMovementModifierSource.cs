using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public interface IMovementModifierSource
    {
        AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input);
    }
}
