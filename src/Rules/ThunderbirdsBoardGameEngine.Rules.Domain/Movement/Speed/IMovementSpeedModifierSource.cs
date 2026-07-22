using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed
{
    public interface IMovementSpeedModifierSource
    {
        CardCode EventCardCode { get; }

        AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input);
    }
}
