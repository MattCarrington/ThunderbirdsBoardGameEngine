using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Mappers
{
    public sealed class MovementResultMapper
    {
        private readonly ILocationDefinitionCatalog _catalog;

        public MovementResultMapper(ILocationDefinitionCatalog catalog)
        {
            _catalog = catalog;
        }

        public MovementResultViewModel ToViewModel(ValidateMovementResponseDto response)
        {
            return new MovementResultViewModel(
                response.IsValid,
                response.ActionPointCost,
                response.SpacesTravelled,
                response.EffectiveTopSpeed ?? 0,
                response.Route
                    .Select(c => _catalog.TryGetByCode(new LocationCode(c), out var location) ? location.DisplayName : c)
                    .ToList(),
                response.Messages.ToList());
        }
    }
}
