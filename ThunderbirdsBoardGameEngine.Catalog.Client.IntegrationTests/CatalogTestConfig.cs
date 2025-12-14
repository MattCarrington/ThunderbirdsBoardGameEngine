namespace ThunderbirdsBoardGameEngine.Catalog.Client.IntegrationTests
{
    public static class CatalogTestConfig
    {
        private const string EnvVarName = "CatalogBaseUrl";
        private const string DefaultUrl = "http://localhost:8080";

        public static string CatalogBaseUrl =>
            Environment.GetEnvironmentVariable(EnvVarName) ?? DefaultUrl;
    }
}
