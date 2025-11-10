using Microsoft.Extensions.Options;
using NSubstitute;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Stubs
{
    public static class JsonOptionsStub
    {
        public static IOptionsMonitor<JsonSerializerOptions> CatalogOptionsMonitor()
        {
            var options = new JsonSerializerOptions();
            CatalogJson.Configure(options);              // ✅ same config as production

            var monitor = Substitute.For<IOptionsMonitor<JsonSerializerOptions>>();
            monitor.Get(CatalogJson.Name).Returns(options);  // name-aware
            monitor.CurrentValue.Returns(options);
            // harmless convenience
            return monitor;
        }
    }
}
