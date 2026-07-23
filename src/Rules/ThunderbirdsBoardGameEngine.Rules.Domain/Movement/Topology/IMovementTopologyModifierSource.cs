using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology
{
    public interface IMovementTopologyModifierSource : ICardSource
    {
        AppliedMovementTopologyModifier ApplyMovementModifier();
    }
}
