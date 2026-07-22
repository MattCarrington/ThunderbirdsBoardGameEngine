using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public record ValidateMovementQuery(ThunderbirdCode ThunderbirdCode, LocationCode StartLocationCode, LocationCode DestinationLocationCode, IReadOnlyList<CardCode> ActiveEventCardCodes)
        : IRequest<ValidateMovementResponse>;
}
