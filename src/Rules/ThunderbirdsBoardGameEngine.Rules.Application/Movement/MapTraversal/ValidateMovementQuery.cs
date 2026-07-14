using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public record ValidateMovementQuery(ThunderbirdCode Thunderbird, LocationCode Start, LocationCode Destination, IReadOnlyList<CardCode> EventCards)
        : IRequest<ValidateMovementResponse>;
}
