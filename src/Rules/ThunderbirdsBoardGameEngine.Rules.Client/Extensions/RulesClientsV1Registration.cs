using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.DelegatingHandlers;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;

namespace ThunderbirdsBoardGameEngine.Rules.Client.Extensions
{
    internal static class RulesClientsV1Registration
    {
        private const string Version = "1.0";

        // configureBase comes from the root method so we don't repeat base address logic
        public static IServiceCollection AddRulesV1Clients(
            this IServiceCollection services,
            Action<IServiceProvider, HttpClient> configureBase)
        {
            IHttpClientBuilder AddV1<TClient, TImpl>()
                where TClient : class
                where TImpl : class, TClient
                => services.AddHttpClient<TClient, TImpl>(configureBase)
                    .AddHttpMessageHandler(() =>
                        new ApiVersionHeaderHandler(Version));

            AddV1<IRescueClient, Clients.V1.RescueClient>();

            return services;
        }
    }
}
