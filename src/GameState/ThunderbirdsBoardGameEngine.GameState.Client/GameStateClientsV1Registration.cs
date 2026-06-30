using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.DelegatingHandlers;
using ThunderbirdsBoardGameEngine.GameState.Client.Clients;

namespace ThunderbirdsBoardGameEngine.GameState.Client
{
    internal static class GameStateClientsV1Registration
    {
        private const string Version = "1.0";

        public static IServiceCollection AddGameStateV1Clients(
            this IServiceCollection services,
            Action<IServiceProvider, HttpClient> configureBase)
        {
            IHttpClientBuilder AddV1<TClient, TImpl>()
                where TClient : class
                where TImpl : class, TClient
                => services.AddHttpClient<TClient, TImpl>(configureBase)
                    .AddHttpMessageHandler(() =>
                        new ApiVersionHeaderHandler(Version));

            AddV1<IGameClient, GameClient>();

            return services;
        }
    }
}
