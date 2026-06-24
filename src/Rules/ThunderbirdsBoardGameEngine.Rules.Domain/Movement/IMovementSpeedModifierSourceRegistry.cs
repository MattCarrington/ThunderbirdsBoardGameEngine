using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public interface IMovementSpeedModifierSourceRegistry
    {
        bool TryGetEventCard(CardCode cardCode, [NotNullWhen(true)] out IMovementSpeedModifierSource? source);
    }
}