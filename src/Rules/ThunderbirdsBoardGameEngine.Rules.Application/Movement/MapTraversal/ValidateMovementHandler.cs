using MediatR;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public class ValidateMovementHandler : IRequestHandler<ValidateMovementQuery, ValidateMovementResponse>
    {
        public Task<ValidateMovementResponse> Handle(ValidateMovementQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
