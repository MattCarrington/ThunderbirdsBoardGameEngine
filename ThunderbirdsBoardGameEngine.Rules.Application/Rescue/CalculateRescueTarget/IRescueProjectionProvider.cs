namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public interface IRescueProjectionProvider
    {
        RescueProjection GetRescueContext(int disasterCardId);
    }
}
