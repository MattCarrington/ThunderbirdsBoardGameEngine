namespace ThunderbirdsBoardGameEngine.SmokeTests
{
    public static class SmokeTestConfig
    {
        private const string EnvVarName = "SMOKE_TEST_BASE_URL";

        public static string SmokeTestBaseUrl =>
            Environment.GetEnvironmentVariable(EnvVarName) ?? throw new InvalidOperationException($"{EnvVarName} is required.");
    }
}