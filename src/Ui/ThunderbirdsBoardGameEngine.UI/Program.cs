using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Services;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Mappers;
using ThunderbirdsBoardGameEngine.UI.Services;

namespace ThunderbirdsBoardGameEngine.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddReferenceData();

            var endpointMode = builder.Configuration["RulesClient:EndpointMode"] ?? "CoHosted";
            if (endpointMode.Equals("CoHosted", StringComparison.OrdinalIgnoreCase))
            {
                // Co-hosted mode: UI and API share the same runtime base URI, so use the host base address (origin + any path base).
                builder.Configuration["RulesClient:BaseAddress"] = builder.HostEnvironment.BaseAddress;
            }
            else if (endpointMode.Equals("External", StringComparison.OrdinalIgnoreCase))
            {
                var configuredBaseAddress = builder.Configuration["RulesClient:BaseAddress"];
                if (string.IsNullOrWhiteSpace(configuredBaseAddress))
                {
                    throw new InvalidOperationException(
                        "RulesClient:BaseAddress must be configured when RulesClient:EndpointMode is set to External.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported RulesClient:EndpointMode '{endpointMode}'. Supported values are CoHosted and External.");
            }

            builder.Services.AddRulesClients(builder.Configuration);

            // Register service layer
            builder.Services.AddScoped<IDisasterCardService, DisasterCardService>();
            builder.Services.AddScoped<ICharacterService, CharacterService>();
            builder.Services.AddScoped<IRescueService, RescueService>();
            builder.Services.AddScoped<IThunderbirdMovementOptionsService, ThunderbirdMovementOptionsService>();
            builder.Services.AddScoped<IMovementLocationOptionsService, MovementLocationOptionsService>();
            builder.Services.AddScoped<IMovementClientService, MovementClientService>();

            builder.Services.AddSingleton<DisasterCardMapper>();
            builder.Services.AddSingleton<CharacterMapper>();
            builder.Services.AddSingleton<MovementResultMapper>();

            await builder.Build().RunAsync();
        }
    }
}
