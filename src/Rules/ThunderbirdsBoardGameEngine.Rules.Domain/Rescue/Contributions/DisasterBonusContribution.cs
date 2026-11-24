using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Contributions
{
    public sealed record DisasterBonusContribution(DisasterBonusKey Key, int Value);
}