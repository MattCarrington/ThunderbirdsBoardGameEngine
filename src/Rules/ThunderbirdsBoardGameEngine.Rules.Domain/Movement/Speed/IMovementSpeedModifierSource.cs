using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed
{
    public interface IMovementSpeedModifierSource : ICardSource
    {
        AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input);
    }
}
