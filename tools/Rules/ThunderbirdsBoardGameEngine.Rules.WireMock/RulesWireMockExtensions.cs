using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1;

namespace ThunderbirdsBoardGameEngine.Rules.WireMock
{
    public static class RulesWireMockExtensions
    {
        public static RescueStub RescueStub(this WireMockHost host)
        {
            return new RescueStub(host.WireMockServer);
        }
    }
}
