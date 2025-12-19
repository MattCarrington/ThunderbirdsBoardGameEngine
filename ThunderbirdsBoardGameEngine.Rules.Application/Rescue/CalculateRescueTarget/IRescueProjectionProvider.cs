namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public interface IRescueProjectionProvider
    {
        RescueProjection GetRescueProjection(int disasterCardId);
    }
}
