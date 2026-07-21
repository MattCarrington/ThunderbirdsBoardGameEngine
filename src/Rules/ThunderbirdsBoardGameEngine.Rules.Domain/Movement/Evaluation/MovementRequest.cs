using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation
{
    public record MovementRequest(ThunderbirdCode Thunderbird, LocationCode Start, LocationCode Destination, IReadOnlyCollection<CardCode> ActiveEventCards);
}
