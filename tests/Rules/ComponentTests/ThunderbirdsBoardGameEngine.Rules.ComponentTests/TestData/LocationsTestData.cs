using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData
{
    public class LocationsTestData
    {
        public static FakeLocationDefinitionCatalog CreateLocations()
        {
            var europe = new ReferenceLocationDefinition(TestLocationCodes.Europe, "Europe", MovementDomain.Earth);
            var asia = new ReferenceLocationDefinition(TestLocationCodes.Asia, "Asia", MovementDomain.Earth);
            var northAmerica = new ReferenceLocationDefinition(TestLocationCodes.NorthAmerica, "North America", MovementDomain.Earth);
            var southAmerica = new ReferenceLocationDefinition(TestLocationCodes.SouthAmerica, "South America", MovementDomain.Earth);
            var atlantic = new ReferenceLocationDefinition(TestLocationCodes.NorthAtlantic, "Atlantic Ocean", MovementDomain.Earth);
            var pacific = new ReferenceLocationDefinition(TestLocationCodes.SouthPacific, "Pacific Ocean", MovementDomain.Earth);
            var australia = new ReferenceLocationDefinition(TestLocationCodes.Australia, "Australia", MovementDomain.Earth);
            var africa = new ReferenceLocationDefinition(TestLocationCodes.Africa, "Africa", MovementDomain.Earth);
            var space = new ReferenceLocationDefinition(TestLocationCodes.GeoStationaryOrbit, "Space", MovementDomain.Space);
            var moon = new ReferenceLocationDefinition(TestLocationCodes.TheMoon, "Moon", MovementDomain.Space);
            var sun = new ReferenceLocationDefinition(TestLocationCodes.TheSun, "Sun", MovementDomain.Space);

            return new FakeLocationDefinitionCatalog(europe, asia, northAmerica, southAmerica, atlantic, pacific, australia, africa, space, moon, sun);
        }
    }
}
