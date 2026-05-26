using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public record ValidateMovementQuery(ThunderbirdCode Thunderbird, LocationCode Start, LocationCode Destination) : IRequest<ValidateMovementResponse>;
}
