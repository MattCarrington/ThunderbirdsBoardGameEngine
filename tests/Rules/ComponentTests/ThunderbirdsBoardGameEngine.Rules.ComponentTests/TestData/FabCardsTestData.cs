using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData
{
    public class FabCardsTestData
    {
        public static FakeFabCardDefinitionCatalog CreateFabCardsCatalog()
        {
            var underwaterSealingUnit = new ReferenceFabCardDefinition(
                code: KnownFabCardCodes.UnderwaterSealingUnit,
                displayName: "Underwater Sealing Unit"
            );

            var astronautSpacewalk = new ReferenceFabCardDefinition(
                code: KnownFabCardCodes.AstronautSpacewalk,
                displayName: "Astronaut Spacewalk"
            );

            var personalHoverjet = new ReferenceFabCardDefinition(
                code: KnownFabCardCodes.PersonalHoverjet,
                displayName: "Personal Hoverjet"
            );

            var remoteControlHoverCamera = new ReferenceFabCardDefinition(
                code: KnownFabCardCodes.RemoteControlHoverCamera,
                displayName: "Remote Control Hover Camera"
            );

            var jeffsOrders = new ReferenceFabCardDefinition(
                code: new CardCode("jeff-s-orders"),
                displayName: "Jeff's Orders"
            );

            return new FakeFabCardDefinitionCatalog(underwaterSealingUnit, astronautSpacewalk, personalHoverjet, remoteControlHoverCamera, jeffsOrders);
        }
    }
}
