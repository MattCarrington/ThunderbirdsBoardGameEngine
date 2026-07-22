using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed
{
    public interface IMovementSpeedModifierSourceRegistry
    {
        bool TryGetEventCard(CardCode cardCode, [NotNullWhen(true)] out IMovementSpeedModifierSource? source);
    }
}