namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Support
{
    public class TestSettings
    {
        public string BaseUrl { get; internal set; }

        public TestSettings()
        {
            BaseUrl = Environment.GetEnvironmentVariable("BASE_URL")
                  ?? throw new InvalidOperationException(
                      "Environment variable BASE_URL is not set. " +
                      "Set it to the base URL of the Blazor app, e.g. http://localhost:5289");
        }
    }
}
