using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed record AppliedMovementSpeedModifier(
        CardCode Card,
        int EffectiveTopSpeed,
        string Message);

}
