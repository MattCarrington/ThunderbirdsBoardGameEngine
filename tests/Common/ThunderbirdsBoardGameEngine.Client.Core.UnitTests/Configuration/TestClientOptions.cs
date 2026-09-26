using ThunderbirdsBoardGameEngine.Client.Core.Configuration;

namespace ThunderbirdsBoardGameEngine.Client.Core.UnitTests.Configuration
{
    public class TestClientOptions : IApiClientOptions
    {
        public string BaseAddress { get; set; } = string.Empty;
    }
}
