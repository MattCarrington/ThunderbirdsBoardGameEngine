using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData
{
    public class CharactersTestData
    {
        public static FakeCharacterDefinitionCatalog CreateCharacterCatalog()
        {
            var scott = new ReferenceCharacterDefinition(
                code: new CharacterCode("scott"),
                displayName: "Scott",
                rescueBonus: new ReferenceCharacterRescueBonus(
                   rescueType: RescueType.Air,
                   value: 2
                )
            );

            var virgil = new ReferenceCharacterDefinition(
                code: new CharacterCode("virgil"),
                displayName: "Virgil",
                rescueBonus: new ReferenceCharacterRescueBonus(
                   rescueType: RescueType.Land,
                   value: 2
                )
            );

            var gordon = new ReferenceCharacterDefinition(
                code: new CharacterCode("gordon"),
                displayName: "Gordon",
                rescueBonus: new ReferenceCharacterRescueBonus(
                   rescueType: RescueType.Sea,
                   value: 3
                )
            );

            return new FakeCharacterDefinitionCatalog(scott, virgil, gordon);
        }
    }
}
