using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public record MovementInput(
        ThunderbirdContribution Thunderbird,
        Topography Topography,
        LocationCode Start,
        LocationCode Destination,
        IReadOnlyCollection<CardCode> EventCards);
}
