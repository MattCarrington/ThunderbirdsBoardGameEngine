using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology
{
    public interface IEffectiveTopographyResolver
    {
        EffectiveTopography Resolve(
            Topography baseTopography,
            IReadOnlyCollection<CardCode> activeEventCards);
    }
}
