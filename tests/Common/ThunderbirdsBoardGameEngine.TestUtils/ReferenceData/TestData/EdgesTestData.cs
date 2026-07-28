using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Fakes;

namespace ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.TestData
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
            var northAtlanticToEurope = new ReferenceMapEdgeDefinition(TestLocationCodes.NorthAtlantic, TestLocationCodes.Europe, MovementDomain.Earth);
            var northAtlanticToAfrica = new ReferenceMapEdgeDefinition(TestLocationCodes.NorthAtlantic, TestLocationCodes.Africa, MovementDomain.Earth);
            var northAtlanticToNorthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.NorthAtlantic, TestLocationCodes.NorthAmerica, MovementDomain.Earth);
            var northAtlanticToSouthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.NorthAtlantic, TestLocationCodes.SouthAmerica, MovementDomain.Earth);
            var pacificToAustralia = new ReferenceMapEdgeDefinition(TestLocationCodes.SouthPacific, TestLocationCodes.Australia, MovementDomain.Earth);
            var pacificToAsia = new ReferenceMapEdgeDefinition(TestLocationCodes.Asia, TestLocationCodes.SouthPacific, MovementDomain.Earth);
            var pacificToNorthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.SouthPacific, TestLocationCodes.NorthAmerica, MovementDomain.Earth);
            var pacificToSouthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.SouthPacific, TestLocationCodes.SouthAmerica, MovementDomain.Earth);
            var pacificToSpace = new ReferenceMapEdgeDefinition(TestLocationCodes.SouthPacific, TestLocationCodes.GeoStationaryOrbit, MovementDomain.Space);
            var spaceToMoon = new ReferenceMapEdgeDefinition(TestLocationCodes.GeoStationaryOrbit, TestLocationCodes.TheMoon, MovementDomain.Space);
            var spaceToSun = new ReferenceMapEdgeDefinition(TestLocationCodes.GeoStationaryOrbit, TestLocationCodes.TheSun, MovementDomain.Space);
            var southAtlanticToAfrica = new ReferenceMapEdgeDefinition(TestLocationCodes.SouthAtlantic, TestLocationCodes.Africa, MovementDomain.Earth);
            var southAtlanticToSouthAmerica = new ReferenceMapEdgeDefinition(TestLocationCodes.SouthAtlantic, TestLocationCodes.SouthAmerica, MovementDomain.Earth);
            var southAtlanticToNorthAtlantic = new ReferenceMapEdgeDefinition(TestLocationCodes.SouthAtlantic, TestLocationCodes.NorthAtlantic, MovementDomain.Earth);

            return new FakeMapEdgeDefinitionCatalog(
                europeToAsia,
                europeToAfrica,
                asiaToAfrica,
                asiaToAustralia,
                africaToAustralia,
                northAmericaToSouthAmerica,
                pacificToAsia,
                northAtlanticToEurope,
                northAtlanticToAfrica,
                northAtlanticToNorthAmerica,
                northAtlanticToSouthAmerica,
                pacificToAustralia,
                pacificToNorthAmerica,
                pacificToSouthAmerica,
                pacificToSpace,
                spaceToMoon,
                spaceToSun,
                southAtlanticToAfrica,
                southAtlanticToSouthAmerica,
                southAtlanticToNorthAtlantic);
        }
    }
}
