using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData
{
    public class EventCardsTestData
    {
        public static FakeEventCardDefinitionCatalog CreateEventCardsCatalog()
        {
            var theHoodInterferes = new ReferenceEventCardDefinition(
                code: KnownEventCardCodes.TheHoodInterferes,
                displayName: "The Hood Interferes");

            var attackOfTheZombites = new ReferenceEventCardDefinition(
                KnownEventCardCodes.AttackOfTheZombites,
                "Attack of the Zombites");

            var usnSentinelMissileStrike = new ReferenceEventCardDefinition(
                KnownEventCardCodes.UsnSentinelMissileStrike,
                "USN Sentinel Missile Strike");

            var rocketMalfunction = new ReferenceEventCardDefinition(
                KnownEventCardCodes.RocketMalfunction,
                "Rocket Malfunction");

            var icelandicVolcanoEruption = new ReferenceEventCardDefinition(
                KnownEventCardCodes.IcelandicVolcanoEruption,
                "Icelandic Volcano Eruption");

            var explosionOnTracyIsland = new ReferenceEventCardDefinition(
                code: new CardCode("explosion-on-tracy-island"),
                displayName: "Explosion on Tracy Island"
            );

            return new FakeEventCardDefinitionCatalog(theHoodInterferes, attackOfTheZombites, usnSentinelMissileStrike, rocketMalfunction, icelandicVolcanoEruption, explosionOnTracyIsland);
        }
    }
}
