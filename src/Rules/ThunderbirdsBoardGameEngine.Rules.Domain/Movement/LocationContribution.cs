using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed record LocationContribution(LocationCode Key, MovementDomain Location);
}
