using ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;

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
