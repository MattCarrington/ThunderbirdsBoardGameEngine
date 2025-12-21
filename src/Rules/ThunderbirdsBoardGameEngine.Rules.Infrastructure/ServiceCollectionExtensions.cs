using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRules(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CalculateRescueTargetHandler).Assembly);

            services.AddSingleton<RescueTargetCalculator>();
            services.AddSingleton<IDisasterContributionLookup, CatalogDisasterContributionLookup>();

            return services;
        }
    }
}
