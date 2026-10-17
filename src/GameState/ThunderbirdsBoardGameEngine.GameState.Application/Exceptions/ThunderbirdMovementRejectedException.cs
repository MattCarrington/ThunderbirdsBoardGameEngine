namespace ThunderbirdsBoardGameEngine.GameState.Application.Exceptions
{
    public sealed class ThunderbirdMovementRejectedException : Exception
    {
        public ThunderbirdMovementRejectedException(IReadOnlyCollection<string> reasons)
            : base("Thunderbird Machine movement was rejected.")
        {
            Reasons = reasons.ToArray();
        }

        public IReadOnlyList<string> Reasons { get; }
    }
}
