using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Registries;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure
{
    /// <summary>
    /// Provides extension methods for registering rule-related services with an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds core rescue rules services and dependencies to the specified service collection.
        /// </summary>
        /// <remarks>This method registers MediatR handlers and singleton services required for rescue
        /// rules processing. Call this method during application startup to enable rescue rules
        /// functionality.</remarks>
        /// <param name="services">The service collection to which the rescue rules services will be added. Cannot be null.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance that was provided, with rescue rules services registered.</returns>
        public static IServiceCollection AddRules(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CalculateRescueTargetHandler).Assembly);

            services.AddSingleton<IValidateMovementResolutionService, ValidateMovementResolutionService>();
            services.AddSingleton<IRouteFinder, BreadthFirstRouteFinder>();

            services.AddSingleton<RescueTargetCalculator>();
            services.AddSingleton<MovementEvaluator>();
            services.AddSingleton<ActionPointCalculator>();

            services.AddSingleton<IDisasterContributionLookup, ReferenceDataDisasterContributionLookup>();
            services.AddSingleton<ICharacterContributionLookup, ReferenceCharacterContributionLookup>();
            services.AddSingleton<ILocationDefinitionLookup, ReferenceLocationDefinitionLookup>();
            services.AddSingleton<IMapEdgeDefinitionLookup, ReferenceMapEdgeDefinitionLookup>();
            services.AddSingleton<IThunderbirdsDefinitionLookup, ReferenceThunderbirdsDefinitionLookup>();
            services.AddSingleton<IBonusModifierSourceRegistry, BonusModifierSourceRegistry>();

            return services;
        }
    }
}
