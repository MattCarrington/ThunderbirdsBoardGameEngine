using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Client.Handlers;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Extensions
{
    internal static class CatalogClientsV1Registration
    {
        private const string Version = "1.0";

        // configureBase comes from the root method so we don't repeat base address logic
        public static IServiceCollection AddCatalogV1Clients(
            this IServiceCollection services,
            Action<IServiceProvider, HttpClient> configureBase)
        {
            IHttpClientBuilder AddV1<TClient, TImpl>()
                where TClient : class
                where TImpl : class, TClient
                => services.AddHttpClient<TClient, TImpl>(configureBase)
                    .AddHttpMessageHandler(() => new ApiVersionHeaderHandler(Version));

            // Register ALL your V1 typed clients here
            AddV1<IDisasterCardsClient, Clients.V1.DisasterCardsClient>();

            return services;
        }
    }
}
