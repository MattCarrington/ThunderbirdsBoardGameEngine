using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Handlers;

namespace ThunderbirdsBoardGameEngine.Client.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClientInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IHttpResponseHandler, DefaultHttpResponseHandler>();
            return services;
        }
    }
}
