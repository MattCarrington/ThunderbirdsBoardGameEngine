using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IValidateMovementResolutionService
    {
        MovementResponse ResolveMovementValidation(MovementRequest request);
    }
}