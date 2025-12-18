namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public interface IRescueContextProvider
    {
        RescueContext GetRescueContext(int disasterCardId);
    }
}
