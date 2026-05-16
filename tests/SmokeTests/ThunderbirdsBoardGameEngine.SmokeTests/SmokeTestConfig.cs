namespace ThunderbirdsBoardGameEngine.SmokeTests
{
    public static class SmokeTestConfig
    {
        private const string EnvVarName = "API_BASE_URL";
        private const string DefaultUrl = "http://localhost:5000";

        public static string ApiBaseUrl =>
            Environment.GetEnvironmentVariable(EnvVarName) ?? DefaultUrl;
    }
}