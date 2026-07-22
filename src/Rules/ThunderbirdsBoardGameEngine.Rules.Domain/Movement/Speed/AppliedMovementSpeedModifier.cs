using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed
{
    public sealed record AppliedMovementSpeedModifier(
        CardCode Card,
        int EffectiveTopSpeed,
        string Message);

}
