namespace ThunderbirdsBoardGameEngine.Api.UserJourneys.Support
{
    public interface IApplicationRestarter
    {
        Task RestartAsync(CancellationToken cancellationToken);
    }
}
