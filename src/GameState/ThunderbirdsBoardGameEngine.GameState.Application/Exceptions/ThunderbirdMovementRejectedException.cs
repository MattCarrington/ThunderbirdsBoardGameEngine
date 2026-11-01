namespace ThunderbirdsBoardGameEngine.GameState.Application.Exceptions
{
    public sealed class ThunderbirdMovementRejectedException : Exception
    {
        public ThunderbirdMovementRejectedException(IReadOnlyCollection<string> reasons, Exception? innerException = null)
            : base("Thunderbird Machine movement was rejected.", innerException)
        {
            Reasons = reasons.ToArray();
        }

        public IReadOnlyList<string> Reasons { get; }
    }
}
