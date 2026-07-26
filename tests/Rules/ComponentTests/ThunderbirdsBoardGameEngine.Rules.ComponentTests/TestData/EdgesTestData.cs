using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData
{
    public class EdgesTestData
    {
        public static FakeMapEdgeDefinitionCatalog CreateEdges()
        {
            var europeToAsia = new ReferenceMapEdgeDefinition(TestLocationCodes.Europe, TestLocationCodes.Asia, MovementDomain.Earth);
            var europeToAfrica = new ReferenceMapEdgeDefinition(TestLocationCodes.Europe, TestLocationCodes.Africa, MovementDomain.Earth);
            var asiaToAfrica = new ReferenceMapEdgeDefinition(TestLocationCodes.Asia, TestLocationCodes.Africa, MovementDomain.Earth);
            var asiaToAustralia = new ReferenceMapEdgeDefinition(TestLocationCodes.Asia, TestLocationCodes.Australia, MovementDomain.Earth);
            var africaToAustralia = new ReferenceMapEdgeDefinition(TestLocationCodes.Africa, TestLocationCodes.Australia, MovementDomain.Earth);
            var northAmericaToSouthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.NorthAmerica, TestLocationCodes.SouthAmerica, MovementDomain.Earth);
            var atlanticToEurope = new ReferenceMapEdgeDefinition(TestLocationCodes.NorthAtlantic, TestLocationCodes.Europe, MovementDomain.Earth);
            var atlanticToAfrica = new ReferenceMapEdgeDefinition(TestLocationCodes.NorthAtlantic, TestLocationCodes.Africa, MovementDomain.Earth);
            var atlanticToNorthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.NorthAtlantic, TestLocationCodes.NorthAmerica, MovementDomain.Earth);
            var atlanticToSouthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.NorthAtlantic, TestLocationCodes.SouthAmerica, MovementDomain.Earth);
            var pacificToAustralia = new ReferenceMapEdgeDefinition(TestLocationCodes.Pacific, TestLocationCodes.Australia, MovementDomain.Earth);
            var pacificToAsia = new ReferenceMapEdgeDefinition(TestLocationCodes.Asia, TestLocationCodes.Pacific, MovementDomain.Earth);
            var pacificToNorthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.Pacific, TestLocationCodes.NorthAmerica, MovementDomain.Earth);
            var pacificToSouthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.Pacific, TestLocationCodes.SouthAmerica, MovementDomain.Earth);
            var pacificToSpace = new ReferenceMapEdgeDefinition(TestLocationCodes.Pacific, TestLocationCodes.Space, MovementDomain.Space);
            var spaceToMoon = new ReferenceMapEdgeDefinition(TestLocationCodes.Space, TestLocationCodes.Moon, MovementDomain.Space);
            var spaceToSun = new ReferenceMapEdgeDefinition(TestLocationCodes.Space, TestLocationCodes.Sun, MovementDomain.Space);

            return new FakeMapEdgeDefinitionCatalog(
                europeToAsia,
                europeToAfrica,
                asiaToAfrica,
                asiaToAustralia,
                africaToAustralia,
                northAmericaToSouthAmerica,
                pacificToAsia,
                asiaToAustralia,
                northAmericaToSouthAmerica,
                atlanticToEurope,
                atlanticToAfrica,
                atlanticToNorthAmerica,
                atlanticToSouthAmerica,
                pacificToAustralia,
                pacificToNorthAmerica,
                pacificToSouthAmerica,
                pacificToSpace,
                spaceToMoon,
                spaceToSun);
        }
    }
}
