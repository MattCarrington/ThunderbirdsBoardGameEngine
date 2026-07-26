using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData
{
    public class ThunderbirdsTestData
    {
        public static FakeThunderbirdDefinitionCatalog CreateThunderbirds()
        {
            var thunderbird1 = new ReferenceThunderbirdDefinition(KnownThunderbirdCodes.Thunderbird1, "Thunderbird 1", MovementDomain.Earth, 3);
            var thunderbird2 = new ReferenceThunderbirdDefinition(KnownThunderbirdCodes.Thunderbird2, "Thunderbird 2", MovementDomain.Earth, 2);
            var thunderbird3 = new ReferenceThunderbirdDefinition(KnownThunderbirdCodes.Thunderbird3, "Thunderbird 3", MovementDomain.Space, 3);
            var thunderbird4 = new ReferenceThunderbirdDefinition(KnownThunderbirdCodes.Thunderbird4, "Thunderbird 4", MovementDomain.Earth, 1);
            var thunderbird5 = new ReferenceThunderbirdDefinition(KnownThunderbirdCodes.Thunderbird5, "Thunderbird 5", MovementDomain.Space, 0);

            return new FakeThunderbirdDefinitionCatalog(thunderbird1, thunderbird2, thunderbird3, thunderbird4, thunderbird5);
        }
    }
}
