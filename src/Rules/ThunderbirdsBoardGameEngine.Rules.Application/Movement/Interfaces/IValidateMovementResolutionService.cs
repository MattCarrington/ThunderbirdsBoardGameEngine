using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IValidateMovementResolutionService
    {
        MovementResponse ResolveMovementValidation(MovementRequest request);
    }
}