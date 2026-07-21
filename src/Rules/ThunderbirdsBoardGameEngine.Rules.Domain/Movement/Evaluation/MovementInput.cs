using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Contributions;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation
{
    public record MovementInput(
        ThunderbirdContribution Thunderbird,
        Topography Topography,
        LocationCode Start,
        LocationCode Destination,
        IReadOnlyCollection<CardCode> EventCards);
}
