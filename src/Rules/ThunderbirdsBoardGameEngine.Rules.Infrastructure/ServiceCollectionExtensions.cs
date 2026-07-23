using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Validators;
using ThunderbirdsBoardGameEngine.Rules.Domain.EventCards;
using ThunderbirdsBoardGameEngine.Rules.Domain.FabCards;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Routing;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups;

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

            services.AddSingleton<IDisasterCatalogLookup, ReferenceDisasterCatalogLookup>();
            services.AddSingleton<ICharacterCatalogLookup, ReferenceCharacterCatalogLookup>();
            services.AddSingleton<ILocationDefinitionLookup, ReferenceLocationDefinitionLookup>();
            services.AddSingleton<IMapEdgeDefinitionLookup, ReferenceMapEdgeDefinitionLookup>();
            services.AddSingleton<IThunderbirdsDefinitionLookup, ReferenceThunderbirdsDefinitionLookup>();
            services.AddSingleton<IFabCardCatalogLookup, ReferenceFabCardCatalogLookup>();
            services.AddSingleton<IEventCardCatalogLookup, ReferenceEventCardCatalogLookup>();

            services.AddSingleton<IEventCardValidator, EventCardValidator>();

            RegisterRescueServices(services);
            RegisterMovementServices(services);
            RegisterMovementSpeedModifierSources(services);
            RegisterMovementTopologyModifierSources(services);

            return services;
        }

        private static void RegisterRescueServices(IServiceCollection services)
        {
            services.AddSingleton<RescueTargetCalculator>();
            services.AddSingleton<ICalculateRescueTargetResolutionService, CalculateRescueTargetResolutionService>();
            services.AddSingleton<ICardBonusModifierSourceRegistry, CardBonusModifierSourceRegistry>();
            services.AddSingleton<ICardRescueModifierSource, TheHoodInterferes>();
            services.AddSingleton<ICardRescueModifierSource, AstronautSpacewalk>();
            services.AddSingleton<ICardRescueModifierSource, PersonalHoverjet>();
            services.AddSingleton<ICardRescueModifierSource, RemoteControlHoverCamera>();
            services.AddSingleton<ICardRescueModifierSource, UnderwaterSealingUnit>();
        }

        private static void RegisterMovementServices(IServiceCollection services)
        {
            services.AddSingleton<IValidateMovementResolutionService, ValidateMovementResolutionService>();
            services.AddSingleton<IRouteFinder, BreadthFirstRouteFinder>();
            services.AddSingleton<MovementEvaluator>();
            services.AddSingleton<ActionPointCalculator>();
        }

        private static void RegisterMovementSpeedModifierSources(IServiceCollection services)
        {
            services.AddSingleton<IMovementSpeedModifierSourceRegistry, MovementSpeedModifierSourceRegistry>();
            services.AddSingleton<IMovementSpeedModifierSource, AttackOfTheZombites>();
            services.AddSingleton<IMovementSpeedModifierSource, UsnSentinelMissileStrike>();
            services.AddSingleton<IMovementSpeedModifierSource, RocketMalfunction>();
        }

        private static void RegisterMovementTopologyModifierSources(IServiceCollection services)
        {
            services.AddSingleton<IEffectiveTopographyResolver, EffectiveTopographyResolver>();
            services.AddSingleton<IMovementTopologyModifierSourceRegistry, MovementTopologyModifierSourceRegistry>();
            services.AddSingleton<IMovementTopologyModifierSource, IcelandicVolcanoEruption>();
        }
    }
}
