namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public class ActionPointCalculator
    {
        public int CalculateActionPoints(int spacesTravelled, int topSpeed)
        {
            if (spacesTravelled < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(spacesTravelled), "Spaces travelled cannot be negative.");
            }

            if (topSpeed <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(topSpeed), "Top Speed cannot be zero or negative");
            }

            return (int)Math.Ceiling((decimal)spacesTravelled / topSpeed);
        }
    }
}
