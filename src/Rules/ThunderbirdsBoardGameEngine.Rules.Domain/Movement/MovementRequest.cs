using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public record MovementRequest(ThunderbirdCode Thunderbird, LocationCode Start, LocationCode Destination, IReadOnlyCollection<CardCode> ActiveEventCards);
}
