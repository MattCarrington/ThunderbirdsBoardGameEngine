using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public class ValidateMovementHandler : IRequestHandler<ValidateMovementQuery, ValidateMovementResponse>
    {
        private readonly ILocationDefinitionLookup _locationDefinitionLookup;
        private readonly IMapEdgeDefinitionLookup _edgeDefinitionLookup;

        public ValidateMovementHandler(ILocationDefinitionLookup locationDefinitionLookup, IMapEdgeDefinitionLookup edgeDefinitionLookup)
        {
            _locationDefinitionLookup = locationDefinitionLookup;
            _edgeDefinitionLookup = edgeDefinitionLookup;
        }

        public Task<ValidateMovementResponse> Handle(ValidateMovementQuery request, CancellationToken cancellationToken)
        {
            var locations = _locationDefinitionLookup.GetAll();
            var edges = _edgeDefinitionLookup.GetAll();

            var topography = new Topography(locations, edges);

            return Task.FromResult(new ValidateMovementResponse(false));
        }
    }
}
