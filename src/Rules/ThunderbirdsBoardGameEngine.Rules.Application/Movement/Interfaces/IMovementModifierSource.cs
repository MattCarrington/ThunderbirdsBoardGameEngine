using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IMovementModifierSource
    {
        AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input);
    }
}
