using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Interfaces
{
    public interface IRescueContextProvider
    {
        RescueContext GetRescueContext(int disasterCardId);
    }
}
