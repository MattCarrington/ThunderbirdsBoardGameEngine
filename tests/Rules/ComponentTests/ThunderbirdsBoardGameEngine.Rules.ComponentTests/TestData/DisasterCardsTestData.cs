using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData
{
    public class DisasterCardsTestData
    {
        public static FakeDisasterDefinitionCatalog CreateDisasterCatalog()
        {
            var sunProbe = new ReferenceDisasterDefinition(
                code: TestDisasterCodes.SunProbe,
                displayName: "Sun Probe",
                difficultyNumber: 11,
                rescueType: RescueType.Space,
                location: TestLocationCodes.TheSun,
                bonuses: [
                    new ReferenceDisasterBonus(new DisasterBonusKey("scott"), 2, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("virgil"), 2, TestLocationCodes.Asia),
                    new ReferenceDisasterBonus(new DisasterBonusKey("transmitter-truck"), 3, TestLocationCodes.Asia)
                ],
                rewards:
                [
                    new ReferenceDisasterReward.PlayerChoice(),
                    new ReferenceDisasterReward.SpecificToken(BonusToken.Logistics)
                ]
            );
            var pitOfPeril = new ReferenceDisasterDefinition(
                code: TestDisasterCodes.PitOfPeril,
                displayName: "Pit of Peril",
                difficultyNumber: 11,
                location: TestLocationCodes.Africa,
                rescueType: RescueType.Land,
                bonuses:
                [
                    new ReferenceDisasterBonus(new DisasterBonusKey("scott"), 2, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("mole"), 3, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("recovery-vehicles"), 2, null)
                ],
                rewards:
                [
                    new ReferenceDisasterReward.PlayerChoice(),
                    new ReferenceDisasterReward.SpecificToken(BonusToken.Determination)
                ]
            );

            var terrorInNewYorkCity = new ReferenceDisasterDefinition(
                code: TestDisasterCodes.TerrorInNewYorkCity,
                displayName: "Terror in New York City",
                difficultyNumber: 11,
                location: TestLocationCodes.NorthAmerica,
                rescueType: RescueType.Sea,
                bonuses:
                [
                    new ReferenceDisasterBonus(new DisasterBonusKey("thunderbird-4"), 2, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("virgil"), 2, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("firefly"), 3, null)
                ],
                rewards:
                [
                    new ReferenceDisasterReward.PlayerChoice(),
                    new ReferenceDisasterReward.SpecificToken(BonusToken.Teamwork)
                ]
            );

            return new FakeDisasterDefinitionCatalog(sunProbe, pitOfPeril, terrorInNewYorkCity);
        }
    }
}