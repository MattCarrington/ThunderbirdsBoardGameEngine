using System.Reflection;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.UnitTests.Domain;

public class DefinitionConstructionBoundaryTests
{
    [Fact]
    public void Definitions_ExposeReadOnlyPropertiesWithoutPublicConstructors()
    {
        // The snapshot is an infrastructure transport envelope, not a catalogue definition.
        var modelTypes = typeof(ReferenceThunderbirdDefinition).Assembly.GetExportedTypes()
            .Where(type => type.Namespace == typeof(ReferenceThunderbirdDefinition).Namespace
                && type != typeof(ReferenceDataSnapshot))
            .ToArray();

        Assert.NotEmpty(modelTypes);
        foreach (var type in modelTypes)
        {
            Assert.Empty(type.GetConstructors());
            Assert.All(type.GetProperties(BindingFlags.Instance | BindingFlags.Public), property =>
            {
                Assert.NotNull(property.GetMethod);
                Assert.Null(property.SetMethod);
            });
        }
    }
}
