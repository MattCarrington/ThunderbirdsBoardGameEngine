using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models;

namespace ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;

/// <summary>
/// Creates reference definitions for automated tests. This assembly is test tooling,
/// not a production construction API. Prefer shared fixtures for standard game data.
/// </summary>
public static class ReferenceDefinitionFactory
{
    public static ReferenceCharacterDefinition Character(
        CharacterCode code,
        string displayName,
        ReferenceCharacterRescueBonus? rescueBonus = null)
    {
        return new(code, displayName, rescueBonus);
    }

    public static ReferenceCharacterRescueBonus CharacterRescueBonus(RescueType rescueType, int value)
    {
        return new(rescueType, value);
    }

    public static ReferenceThunderbirdDefinition Thunderbird(
        ThunderbirdCode code,
        string displayName,
        MovementDomain domain = MovementDomain.Earth,
        int topSpeed = 0,
        int characterCapacity = 1)
    {
        return new(code, displayName, domain, topSpeed, characterCapacity);
    }

    public static ReferenceLocationDefinition Location(
        LocationCode code,
        string displayName,
        MovementDomain domain = MovementDomain.Earth)
    {
        return new(code, displayName, domain);
    }

    public static ReferencePodVehicleDefinition PodVehicle(PodVehicleCode code, string displayName)
    {
        return new(code, displayName);
    }

    public static ReferenceMapEdgeDefinition MapEdge(
        LocationCode edge1, LocationCode edge2, MovementDomain edgeType)
    {
        return new(edge1, edge2, edgeType);
    }

    public static ReferenceFabCardDefinition FabCard(CardCode code, string displayName)
    {
        return new(code, displayName);
    }

    public static ReferenceEventCardDefinition EventCard(CardCode code, string displayName)
    {
        return new(code, displayName);
    }

    public static ReferenceDisasterDefinition Disaster(
        CardCode code,
        string displayName,
        int difficultyNumber,
        LocationCode location,
        RescueType rescueType,
        IReadOnlyList<ReferenceDisasterBonus> bonuses,
        IReadOnlyList<ReferenceDisasterReward> rewards)
    {
        return new(code, displayName, difficultyNumber, location, rescueType, bonuses, rewards);
    }

    public static ReferenceDisasterBonus DisasterBonus(DisasterBonusKey key, int value, LocationCode? location)
    {
        return new(key, value, location);
    }

    public static ReferenceDisasterReward.SpecificToken SpecificToken(BonusToken token)
    {
        return new(token);
    }

    public static ReferenceDisasterReward.PlayerChoice PlayerChoice()
    {
        return new();
    }

    public static DisasterBonusKeyDefinition DisasterBonusKey(DisasterBonusKey key, string displayName)
    {
        return new(key, displayName);
    }
}
