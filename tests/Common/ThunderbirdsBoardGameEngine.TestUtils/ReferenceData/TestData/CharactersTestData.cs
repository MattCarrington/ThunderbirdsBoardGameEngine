using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Fakes;

namespace ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.TestData
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

            var john = new ReferenceCharacterDefinition(
                code: new CharacterCode("john"),
                displayName: "John",
                rescueBonus: new ReferenceCharacterRescueBonus(
                   rescueType: RescueType.Space,
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

            var alan = new ReferenceCharacterDefinition(
                code: new CharacterCode("alan"),
                displayName: "Alan",
                rescueBonus: new ReferenceCharacterRescueBonus(
                   rescueType: RescueType.Space,
                   value: 2
                )
            );

            var ladyPenelope = new ReferenceCharacterDefinition(
                code: new CharacterCode("lady-penelope"),
                displayName: "Lady Penelope",
                rescueBonus: null
            );

            return new FakeCharacterDefinitionCatalog(scott, virgil, gordon, john, alan, ladyPenelope);
        }
    }
}
