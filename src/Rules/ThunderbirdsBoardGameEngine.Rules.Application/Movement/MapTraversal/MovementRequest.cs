using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public record MovementRequest(ThunderbirdContribution Thunderbird, Topography Topography);
}
