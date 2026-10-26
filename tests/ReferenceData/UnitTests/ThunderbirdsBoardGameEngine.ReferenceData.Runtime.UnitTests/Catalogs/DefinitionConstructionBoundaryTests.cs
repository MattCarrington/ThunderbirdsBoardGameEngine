using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Catalogs;

public class DefinitionConstructionBoundaryTests
{
    [Fact]
    public void BonusKeyDefinition_ExposesReadOnlyPropertiesWithoutPublicConstructors()
    {
        var type = typeof(DisasterBonusKeyDefinition);
        Assert.Empty(type.GetConstructors());
        Assert.All(type.GetProperties(), property => Assert.Null(property.SetMethod));
    }
}
