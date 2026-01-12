namespace ThunderbirdsBoardGameEngine.Rules.Client.IntegrationTests
{
    public static class RulesTestConfig
    {
        private const string EnvVarName = "RulesBaseUrl";
        private const string DefaultUrl = "http://localhost:8080";

        public static string RulesBaseUrl =>
            Environment.GetEnvironmentVariable(EnvVarName) ?? DefaultUrl;
    }
}
