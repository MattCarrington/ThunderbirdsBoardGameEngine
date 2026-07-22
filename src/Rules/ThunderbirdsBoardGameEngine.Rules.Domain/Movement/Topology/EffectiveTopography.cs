namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology
{
    public sealed record EffectiveTopography(
        Topography Value,
        IReadOnlyCollection<string> Messages);
}
