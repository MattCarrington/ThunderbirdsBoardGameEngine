using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;

namespace ThunderbirdsBoardGameEngine.Api.Mappers.Rules.V1
{
    public static class MovementMappingExtensions
    {
        public static ValidateMovementQuery ToQuery(this ValidateMovementRequestDto request, string thunderbirdCode)
        {
            return new ValidateMovementQuery
            (
                ThunderbirdCode: new ThunderbirdCode(thunderbirdCode),
                StartLocationCode: new LocationCode(request.StartLocation),
                DestinationLocationCode: new LocationCode(request.DestinationLocation),
                ActiveEventCardCodes: request.ActiveEventCardKeys.Select(c => new CardCode(c)).ToList()
            );
        }

        public static ValidateMovementResponseDto ToDto(this ValidateMovementResponse response)
        {
            return new ValidateMovementResponseDto
            {
                IsValid = response.IsValid,
                ActionPointCost = response.ActionPointCost,
                SpacesTravelled = response.SpacesTravelled,
                Messages = response.Messages.ToList(),
                Route = response.Route.Select(locationCode => locationCode.Value).ToList(),
                ThunderbirdTopSpeed = response.ThunderbirdTopSpeed,
                EffectiveTopSpeed = response.EffectiveTopSpeed
            };
        }
    }
}
