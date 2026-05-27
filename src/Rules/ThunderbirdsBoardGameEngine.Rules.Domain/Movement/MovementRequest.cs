using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public record MovementRequest(ThunderbirdContribution Thunderbird, Topography Topography, LocationCode Start, LocationCode Destination);
}
