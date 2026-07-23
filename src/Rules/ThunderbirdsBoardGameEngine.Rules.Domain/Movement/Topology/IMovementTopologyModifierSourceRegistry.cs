using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology
{
    public interface IMovementTopologyModifierSourceRegistry
    {
        bool TryGetEventCard(CardCode cardCode, [NotNullWhen(true)] out IMovementTopologyModifierSource? source);
    }
}
